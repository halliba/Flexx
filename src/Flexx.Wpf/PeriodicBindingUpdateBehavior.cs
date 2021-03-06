﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace Flexx.Wpf
{
    public class PeriodicBindingUpdateBehavior : Behavior<DependencyObject>
    {
        public TimeSpan Interval { get; set; }

        public DependencyProperty Property { get; set; }

        public PeriodicBindingUpdateMode Mode { get; set; } = PeriodicBindingUpdateMode.UpdateTarget;

        private WeakTimer _timer;
        private TimerCallback _timerCallback;

        protected override void OnAttached()
        {
            if (Interval == null) throw new ArgumentNullException(nameof(Interval));
            if (Property == null) throw new ArgumentNullException(nameof(Property));
            //Save a reference to the callback of the timer so this object will keep the timer alive but not vice versa.
            _timerCallback = s =>
            {
                try
                {
                    switch (Mode)
                    {
                        case PeriodicBindingUpdateMode.UpdateTarget:
                            Dispatcher.Invoke(() => BindingOperations.GetBindingExpression(AssociatedObject, Property)?.UpdateTarget());
                            break;
                        case PeriodicBindingUpdateMode.UpdateSource:
                            Dispatcher.Invoke(() => BindingOperations.GetBindingExpression(AssociatedObject, Property)?.UpdateSource());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (TaskCanceledException) { }//This exception will be thrown when application is shutting down.
            };
            _timer = new WeakTimer(_timerCallback, null, Interval, Interval);

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            _timer.Dispose();
            _timerCallback = null;
            base.OnDetaching();
        }
    }

    public enum PeriodicBindingUpdateMode
    {
        UpdateTarget,
        UpdateSource
    }

    /// <summary>
    /// Wraps up a <see cref="Timer"/> with only a <see cref="WeakReference"/> to the callback so that the timer does not prevent GC from collecting the object that uses this timer.
    /// Your object must hold a reference to the callback passed into this timer.
    /// </summary>
    public class WeakTimer : IDisposable
    {
        private readonly Timer _timer;
        private readonly WeakReference<TimerCallback> _weakCallback;

        public WeakTimer(TimerCallback callback)
        {
            _timer = new Timer(OnTimerCallback);
            _weakCallback = new WeakReference<TimerCallback>(callback);
        }

        public WeakTimer(TimerCallback callback, object state, int dueTime, int period)
        {
            _timer = new Timer(OnTimerCallback, state, dueTime, period);
            _weakCallback = new WeakReference<TimerCallback>(callback);
        }

        public WeakTimer(TimerCallback callback, object state, TimeSpan dueTime, TimeSpan period)
        {
            _timer = new Timer(OnTimerCallback, state, dueTime, period);
            _weakCallback = new WeakReference<TimerCallback>(callback);
        }

        public WeakTimer(TimerCallback callback, object state, uint dueTime, uint period)
        {
            _timer = new Timer(OnTimerCallback, state, dueTime, period);
            _weakCallback = new WeakReference<TimerCallback>(callback);
        }

        public WeakTimer(TimerCallback callback, object state, long dueTime, long period)
        {
            _timer = new Timer(OnTimerCallback, state, dueTime, period);
            _weakCallback = new WeakReference<TimerCallback>(callback);
        }

        private void OnTimerCallback(object state)
        {
            if (_weakCallback.TryGetTarget(out TimerCallback callback))
                callback(state);
            else
                _timer.Dispose();
        }

        public bool Change(int dueTime, int period)
        {
            return _timer.Change(dueTime, period);
        }
        public bool Change(TimeSpan dueTime, TimeSpan period)
        {
            return _timer.Change(dueTime, period);
        }

        public bool Change(uint dueTime, uint period)
        {
            return _timer.Change(dueTime, period);
        }

        public bool Change(long dueTime, long period)
        {
            return _timer.Change(dueTime, period);
        }

        public bool Dispose(WaitHandle notifyObject)
        {
            return _timer.Dispose(notifyObject);
        }
        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}