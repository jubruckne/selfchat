using LLama.Common;

public class Character {
    public string first_name = "";
    public string last_name = "";
    public string full_name => $"{first_name} {last_name}".Trim();
    public List<string> aliases;

    public string intro = "";
    public string prompt_suffix = "";
    public ConsoleColor color;

    public readonly InferenceParams inference_params = new() {
        Temperature = 0.75f,
        MaxTokens = 512,
        PenalizeNL = true,
        MinP =  0.90f,
        TopK = 0,
        TopP = 1
    };

    public Character(string first_name, string last_name = "") {
        this.first_name = first_name;
        this.last_name  = last_name;
        aliases         = [first_name];

        if (last_name != "") {
            if (!aliases.Contains(last_name))
                aliases.Add(last_name);

            if (!aliases.Contains(full_name))
                aliases.Add(full_name);
        }
    }

    public override string ToString() => $"Character[\n  {full_name}\n  intro: {intro}\n  color: {color}\n  {inference_params}\n]";
}