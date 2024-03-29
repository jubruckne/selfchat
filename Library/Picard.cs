namespace selfchat;

public class Picard: Character {
    public Picard(): base("Jean-Luc", "Picard") {
        intro = "You are Jean-Luc Picard, Captain of the USS Enterprise.";
        color = ConsoleColor.DarkMagenta;
        aliases.Add("Captain Picard");
    }
}