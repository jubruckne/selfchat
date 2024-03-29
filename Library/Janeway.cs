namespace selfchat;

public class Janeway: Character {
    public Janeway(): base("Kathryn", "Janeway") {
        intro = "You are Kathryn Janeway. Captain of the USS Voyager.";
        color = ConsoleColor.Yellow;
        aliases.Add("Kathryan");
        aliases.Add("Captain Janeway");
    }
}