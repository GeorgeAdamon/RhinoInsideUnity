using Rhino.Compute;

namespace RhinoInsideUnity
{
    public static class RhinoComputeAuthorization
    {
        public static string Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwIjoiUEtDUyM3IiwiYyI6IkFFU18yNTZfQ0JDIiwiYjY0aXYiOiJvUWhFSVN2S1VBUnd2UGx0WWdZMFpBPT0iLCJiNjRjdCI6Ijl1ZUg4ZXRCcmhnRTh3QjNCcVRvOWZmT0NmUHNWcEt4blVKRDBnZFYxUkMxRmNGdDE4NlhjdHNNM21wWUEvbkUyNzVMSGl5UzJCWjVRcEtLZGJ6UGxtRllQa2xSM09lUXl4MkhHYlBpS25XZVJRVVRJbFhGbHVHMzhMNVdvbnVncFFRNzZyN1RuSGY4T0lVb09TeXM5WERTSHVwdFJGaTg3TmZzZktTZ3d6WVBNamZLWGNKUUJuTDdTZng2bERnM1NKakJrNzF1MnNmcnRzcjVGK1JzSEE9PSIsImlhdCI6MTU0NzQzMDc4NH0.OCqG0TR-eGiBYHyCtZysxmLZmVnKuX-WHuI_etCo8ow";


        public static void RequestAuthorization()
        {
            ComputeServer.AuthToken = Token;
        }
    }
}
