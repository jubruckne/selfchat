using LLama.Native;
using LLama.Sampling;

public class Sampler: ISamplingPipeline {
    public delegate LLamaToken SampleDelegate(SafeLLamaContextHandle ctx, Span<float> logits,
        ReadOnlySpan<LLamaToken> last_tokens);

    public delegate LLamaToken SampleCandidatesDelegate(LLamaTokenDataArray candidates);

    public static Sampler make(SampleCandidatesDelegate sample) => new Sampler(sample);

    private readonly SampleCandidatesDelegate on_sample;

    private Sampler(SampleCandidatesDelegate on_sample) {
        this.on_sample = on_sample;
    }

    public void Dispose() => GC.SuppressFinalize(this);

    LLamaToken ISamplingPipeline.Sample(SafeLLamaContextHandle ctx, Span<float> logits, ReadOnlySpan<LLamaToken> lastTokens) {
        LLamaTokenDataArray candidates = LLamaTokenDataArray.Create((ReadOnlySpan<float>) logits);
        return on_sample(candidates);
    }

    void ISamplingPipeline.Accept(SafeLLamaContextHandle ctx, LLamaToken token) {
        Console.WriteLine("Accept: " + token);
    }

    public void Reset() {
        throw new NotImplementedException();
    }

    ISamplingPipeline ISamplingPipeline.Clone() {
        throw new NotImplementedException();
    }
}