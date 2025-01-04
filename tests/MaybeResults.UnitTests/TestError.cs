namespace MaybeResults.Test;

// The [None] attribute will trigger the NoneGenerator which will create
// a Custom None type that we can return in case of errors.
[None]
public sealed partial record TestError;
