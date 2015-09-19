using System.Collections;
using System.Collections.Generic;

namespace Arma3BE.Server.Models
{
    public abstract class StateList<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _data;

        protected StateList(IEnumerable<T> data)
        {
            _data = data;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }
    }
}