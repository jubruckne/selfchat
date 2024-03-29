using LLama;
using LLama.Common;
using LLama.Native;
using selfchat;

public static class Chat {
    private static string model_path = null!;
    private static LLamaWeights weights = null!;
    private static ModelParams parameters = null!;

    public static void init(string model = "/Users/julia/models/WestLake-10.7B-v2-Q5_K_M-imat.gguf") {
        Chat.model_path = model;
        parameters = new ModelParams(model_path) {
            ContextSize = 4096,
            Threads = 2,
            BatchThreads = 6,
            GpuLayerCount = 99,
            BatchSize = 128,
            //TypeK = GGMLType.GGML_TYPE_Q8_0,
            //TypeV = GGMLType.GGML_TYPE_Q8_0,
        };

        weights = LLamaWeights.LoadFromFile(parameters);
    }

    public static void test() {
        var ctx = new LLamaContext(weights, parameters);

        var batch = new LLamaBatch();


        var tokens = ctx.Tokenize("hello", true, true);
        foreach (var t in tokens)
            batch.AddRange(tokens, 0, LLamaSeqId.Zero, true);

        if (ctx.Decode(batch) != DecodeResult.Ok)
            throw new Exception();

        for (int i = 0; i < 100; i++)
            ctx.Sample(Sampler.make(Sample), tokens);

        LLamaToken Sample(LLamaTokenDataArray candidates) {
            candidates.Temperature(ctx.NativeHandle, 0.5f);
            candidates.TopK(ctx.NativeHandle, 10);
            return candidates.data.Span[0].id;
        }

        ctx.Dispose();
    }

    public static void run() {
        var executor = new StatelessExecutor(weights, parameters);

        Console.WriteLine();

        /*f
        var story = new Story(
            executor,
            "A discussion between <character_names> about the the Galaxy Class starship design mistakes and lessons learned for future designs.",
            new Moderator {inference_params = { Temperature = 0.25f}},
            new Picard {inference_params = { Temperature = 0.5f}},
            new Janeway {inference_params = { Temperature = 1.0f}}
);
*/
        var story = new Story(
                              executor,
                              "A discussion between <character_names> about the state of the OpenGL API.",
                              new Moderator {inference_params = { Temperature = 0.25f}},
                              new Student() {inference_params   = { Temperature = 1.00f}},
                              new Carmack() {inference_params    = { Temperature = 0.6f}}
                             );

        Thread.Sleep(5250);

        NativeApi.llama_log_set((level, message) => {
            if(level is LLamaLogLevel.Warning or LLamaLogLevel.Error)
                Console.WriteLine(message);
        });

        Console.WriteLine($"\n{model_path} loaded.");

        story.run().Wait();
    }
}