using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BlockingQueue
{
    public class BlockingQueue<T>
    {
        Queue<T> _queue = new Queue<T>();
        Semaphore _sem = new Semaphore(0, Int32.MaxValue);

        private int QueueCount()
        {
            int nCount = 0;

            lock (_queue)
            {
                nCount = _queue.Count;
            }

            return nCount;
        }

        public int Count
        {
            get
            {
                return QueueCount();
            }
        }

        public void Enqueue(T item)
        {
            lock (_queue)
            {
                _queue.Enqueue(item);
            }

            _sem.Release();
        }

        public void EnqueueAll(T[] pBuf, int num, int offset)
        {
            lock (_queue)
            {
                for (int i = 0; i < num; ++i)
                {
                    _queue.Enqueue(pBuf[i + offset]);
                }
            }

            _sem.Release(num);
        }

        public bool Dequeue(int timeout, ref T rValue)
        {
            if (_sem.WaitOne(timeout, true) == false)
                return false;                           // Timeout

            lock (_queue)
            {
                if (_queue.Count > 0)
                    rValue = _queue.Dequeue();
                else
                    return false;
            }

            return true;
        }

        public bool DequeueAll(int timeout, T[] pBuf, int num)
        {
            for (int i = 0; i < num; ++i)
            {
                if (_sem.WaitOne(timeout, true) == false)
                    return false;                           // Timeout
                lock (_queue)
                {
                    if (_queue.Count > 0)
                        pBuf[i] = _queue.Dequeue();
                    else
                        return false;
                }
            }

            return true;
        }

        public bool Element(int timeout, int nIndex, ref T rValue)
        {
            if (_sem.WaitOne(timeout, true) == false)
                return false;                           // Timeout

            int index = nIndex;

            if (nIndex < 0) 
                index = 0;
            else if (nIndex >= _queue.Count)
                index = _queue.Count - 1;

            lock (_queue)
            {
                rValue = _queue.ElementAt(index);
            }

            return true;
        }

        public void Clear()
        {
            _sem.Release();

            _sem = new Semaphore(0, Int32.MaxValue);

            lock (_queue)
            {
                _queue.Clear();
            }
        }
    }
}
