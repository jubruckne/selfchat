using System.Text;
using LLama.Abstractions;

public class Story {
    private readonly string topic;
    private readonly Character moderator;
    private readonly Dictionary<string, Character> characters;
    private readonly StringBuilder history;
    private readonly ILLamaExecutor executor;

    public Story(ILLamaExecutor executor, string topic, Character moderator, params Character[] characters) {
        this.executor = executor;
        this.moderator = moderator;
        this.characters = new();
        List<string> stop_words = [];

        stop_words.Add(moderator.full_name + ":");

        foreach (var c in characters) {
            this.characters.Add(c.first_name, c);
            foreach(var alias in c.aliases)
                stop_words.Add(alias + ":");
        }

        stop_words.Add((" -------"));
        stop_words.Add((" Best regards"));

        string participants = "";
        foreach (var c in characters[0..^1]) {
            participants += c.full_name + ", ";
        }

        participants = participants[..^2] + " and " + characters[^1].full_name;
        this.topic = topic.Replace("<character_names>", participants);
        this.history = new(this.topic);

        moderator.inference_params.AntiPrompts = stop_words;
        foreach (var c in characters)
            c.inference_params.AntiPrompts = stop_words;

        Console.WriteLine(moderator.inference_params.AntiPrompts);
    }

    public Character this[string name] => characters[name];

    public async Task run() {
        Console.Clear();

        Console.WriteLine(topic);
        Console.WriteLine();

        while (true) {
            bool success;
            string response;

            foreach (var person in characters.Values) {
                (success, response) = await prompt(person);
                if (success) {
                    history.Append($"{person.first_name}: {response}\n");
                    // executor.Context.SaveState($"{person.name}.state");
                } else {
                    history.Append($"{moderator.first_name}: {person.first_name}?");

                    (success, response) = await prompt(person);
                    if (success) {
                        history.Append($"{person.first_name}: {response}\n");
                    } else {
                        Console.WriteLine("****** error ******");
                        return;
                    }
                }
            }

            (success, response) = await prompt(moderator);
            if (success && response.Length > 9) {
                history.Append($"{moderator.first_name}: {response}\n");
            } else {
                Console.Write("\r");
            }
        }
    }

    private async Task<(bool success, string text)> prompt(Character character) {
        var old_color = Console.ForegroundColor;

        var builder = new StringBuilder();
        Console.Write($"{character.first_name}: ");

        var p = get_prompt(character);

//        Console.ForegroundColor = ConsoleColor.Gray;
//        Console.WriteLine($"\n{p}\n");
//        Console.ForegroundColor = old_color;

        Console.ForegroundColor = character.color;

        int count = 0;

        await foreach (var result in executor.InferAsync(p, character.inference_params)) {
            var output = result.Replace('\n', ' ').Replace('\r', ' ').Replace("  ", " ");
            if(count == 0) output = output.TrimStart();

            Console.Write(output);
            count += output.Length;
            builder.Append(output);
        }

        var s = builder.ToString().Trim();
        while (s.EndsWith('\n')) {
            s = s.Substring(0, s.Length - 1).Trim();
        }

        foreach(var stop in character.inference_params.AntiPrompts)
            if (s.EndsWith(stop))
                s = s.Substring(0, s.Length - stop.Length);

        s = s.Replace('\ufffd', ' ');
        s = s.Replace("  ", " ");
        s = s.Trim();

        while (s.EndsWith('\n')) {
            s = s.Substring(0, s.Length - 1).Trim();
        }

        for (var i = 0; i < count - s.Length; ++i) {
            Console.Write("\x1B[1D"); // Move the cursor one unit to the left
            Console.Write("\x1B[1P"); // Delete the character
        }

        if(s.Length > 5) {
            Console.Write("\n");
        } else {
            for (var i = 0; i < character.first_name.Length + 2; ++i) {
                Console.Write("\x1B[1D"); // Move the cursor one unit to the left
                Console.Write("\x1B[1P"); // Delete the character
            }
        }

        Console.ForegroundColor = old_color;

        return (s.Length > 5, s);
    }

    public string get_prompt(Character character) {
        StringBuilder p = new();
        p.AppendLine(character.intro);
        p.AppendLine(history.ToString());
        if(character.prompt_suffix.Length != 0)
            p.AppendLine(character.prompt_suffix);
        p.AppendLine($"{character.first_name}: ");
        return p.ToString();
    }
}