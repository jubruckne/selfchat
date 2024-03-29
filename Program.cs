using LLama.Native;

NativeLibraryConfig
    .Instance
    .WithCuda()
    .WithLogs(true);

NativeApi.llama_empty_call();

Chat.init();
Chat.run();
Chat.test();