namespace Exlibris.Core;

public class CompositeDisposable : AbstractDisposable
{
    private readonly List<IDisposable> disposables = new();

    public T Add<T>(T disposable) where T : IDisposable
    {
        disposables.Add(disposable);
        return disposable;
    }

    public void AddAction(Action action)
        => disposables.Add(new DisposableImpl(action));

    public void Add(IEnumerable<IDisposable> disposables)
    {
        foreach(var disposable in disposables)
        {
            this.disposables.Add(disposable);
        }
    }

    public override void OnDisposing()
    {
        foreach (var disposable in disposables.Distinct())
        {
            try
            {
                if (disposable is AbstractDisposable ad)
                {
                    ad.OnDisposing();
                }
                else
                {
                    disposable.Dispose();
                }
            }
            catch { /* ignore */ }

        }
        disposables.Clear();
    }

    private class DisposableImpl : IDisposable
    {
        private readonly Action action;

        public DisposableImpl(Action action)
        {
            this.action = CallOnce.New(action);
        }

        public void Dispose() => action();
    }
}
