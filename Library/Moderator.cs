namespace selfchat;

public class Moderator: Character {
    public Moderator(): base("Moderator") {
        first_name = "Moderator";

        intro =
            "You are the moderator. Your job is to ensure the discussion keeps on topic. There will be no Q&A today. You only interrupt if you think it's required.";
        prompt_suffix =
            "[Time left: 57 minutes. We still have about 30 minutes before starting Q&A. Make sure the conversation is productive, but do not interrupt unless necessary. Try to keep them continue the conversation, don't let it end!]";
        color = ConsoleColor.Green;
    }
}