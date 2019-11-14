using System.Collections.Generic;

namespace SqlParser.Parsing
{
    /// <summary>
    /// Sequence which keeps track of all items and can rewind the entire batch at once
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WindowSequence<T> : ISequence<T>
    {
        private readonly Stack<T> _window;
        private readonly ISequence<T> _inner;

        public WindowSequence(ISequence<T> inner)
        {
            _inner = inner;
            _window = new Stack<T>();
        }

        public void PutBack(T t)
        {
            if (_window.Peek().Equals(t))
                _window.Pop();
            _inner.PutBack(t);
        }

        public T Peek() => _inner.Peek();

        public bool IsAtEnd => _inner.IsAtEnd;

        public T GetNext()
        {
            var token = _inner.GetNext();
            _window.Push(token);
            return token;
        }

        public void Rewind()
        {
            while (_window.Count > 0)
                _inner.PutBack(_window.Pop());
        }

        public Location CurrentLocation => _inner.CurrentLocation;
    }
}