namespace selfchat;

public class Student: Character {
    public Student(): base("Student") {
        intro = "You are a student, eager to learn from your colleague. Challenge your opponent with deep and difficult technical questions.";
        color = ConsoleColor.DarkMagenta;
    }
}