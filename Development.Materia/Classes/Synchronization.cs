#region "imports"

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Windows.Forms;

#endregion

namespace Development.Materia
{

    #region "Action"

    /// <summary>
    /// Encapsulates a method that has 5 parameters and does not return any value.
    /// </summary>
    /// <typeparam name="T1">1st parameter's value type.</typeparam>
    /// <typeparam name="T2">2nd parameter's value type.</typeparam>
    /// <typeparam name="T3">3rd parameter's value type.</typeparam>
    /// <typeparam name="T4">4th parameter's value type.</typeparam>
    /// <typeparam name="T5">5th parameter's value type.</typeparam>
    /// <param name="arg1">Invoked method's 1st parameter value.</param>
    /// <param name="arg2">Invoked method's 2nd parameter value.</param>
    /// <param name="arg3">Invoked method's 3rd parameter value.</param>
    /// <param name="arg4">Invoked method's 4th parameter value.</param>
    /// <param name="arg5">Invoked method's 5th parameter value.</param>
    public delegate void Action<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    /// <summary>
    /// Encapsulates a method that has 6 parameters and does not return any value.
    /// </summary>
    /// <typeparam name="T1">1st parameter's value type.</typeparam>
    /// <typeparam name="T2">2nd parameter's value type.</typeparam>
    /// <typeparam name="T3">3rd parameter's value type.</typeparam>
    /// <typeparam name="T4">4th parameter's value type.</typeparam>
    /// <typeparam name="T5">5th parameter's value type.</typeparam>
    /// <typeparam name="T6">6th parameter's value type.</typeparam>
    /// <param name="arg1">Invoked method's 1st parameter value.</param>
    /// <param name="arg2">Invoked method's 2nd parameter value.</param>
    /// <param name="arg3">Invoked method's 3rd parameter value.</param>
    /// <param name="arg4">Invoked method's 4th parameter value.</param>
    /// <param name="arg5">Invoked method's 5th parameter value.</param>
    /// <param name="arg6">Invoked method's 6th parameter value.</param>
    public delegate void Action<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

    #endregion

    #region "Func"

    /// <summary>
    ///  Encapsulates a method that has 5 parameters and returns a value of the type specified by the TResult parameter.
    /// </summary>
    /// <typeparam name="T1">1st parameter's value type.</typeparam>
    /// <typeparam name="T2">2nd parameter's value type.</typeparam>
    /// <typeparam name="T3">3rd parameter's value type.</typeparam>
    /// <typeparam name="T4">4th parameter's value type.</typeparam>
    /// <typeparam name="T5">5th parameter's value type.</typeparam>
    /// <typeparam name="TResult">Invoking method's return value type.</typeparam>
    /// <param name="arg1">Invoked method's 1st parameter value.</param>
    /// <param name="arg2">Invoked method's 2nd parameter value.</param>
    /// <param name="arg3">Invoked method's 3rd parameter value.</param>
    /// <param name="arg4">Invoked method's 4th parameter value.</param>
    /// <param name="arg5">Invoked method's 5th parameter value.</param>
    /// <returns></returns>
    public delegate TResult Func<T1, T2, T3, T4, T5, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    /// <summary>
    /// Encapsulates a method that has 6 parameters and returns a value of the type specified by the TResult parameter.
    /// </summary>
    /// <typeparam name="T1">1st parameter's value type.</typeparam>
    /// <typeparam name="T2">2nd parameter's value type.</typeparam>
    /// <typeparam name="T3">3rd parameter's value type.</typeparam>
    /// <typeparam name="T4">4th parameter's value type.</typeparam>
    /// <typeparam name="T5">5th parameter's value type.</typeparam>
    /// <typeparam name="T6">6th parameter's value type.</typeparam>
    /// <typeparam name="TResult">Invoking method's return value type.</typeparam>
    /// <param name="arg1">Invoked method's 1st parameter value.</param>
    /// <param name="arg2">Invoked method's 2nd parameter value.</param>
    /// <param name="arg3">Invoked method's 3rd parameter value.</param>
    /// <param name="arg4">Invoked method's 4th parameter value.</param>
    /// <param name="arg5">Invoked method's 5th parameter value.</param>
    /// <param name="arg6">Invoked method's 6th parameter value.</param>
    /// <returns></returns>
    public delegate TResult Func<T1, T2, T3, T4, T5, T6, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

    #endregion

    /// <summary>
    /// Synchronization method class.
    /// </summary>
    public static class Synchronization
    {

        /// <summary>
        /// End the progressing state of a specified progressbar object asynchronously.
        /// </summary>
        /// <param name="progressbar">Progress bar object to synchronize</param>
        public static void EndProgress(object progressbar)
        {
            if (progressbar != null)
            {
                int _max = 0;
                if (Materia.PropertyExists(progressbar, "Maximum")) _max = Materia.GetPropertyValue<int>(progressbar, "Maximum");
                else
                {
                    if (Materia.PropertyExists(progressbar, "Max")) _max = Materia.GetPropertyValue<int>(progressbar, "Max");
                    else
                    {
                        if (Materia.PropertyExists(progressbar, "MaxValue")) _max = Materia.GetPropertyValue<int>(progressbar, "MaxValue");
                        else _max = 0;
                    }
                }

                if (_max > 0 && Materia.PropertyExists(progressbar, "Value"))
                {
                    int _curvalue = Materia.GetPropertyValue<int>(progressbar, "Value");

                    while (_curvalue < _max)
                    {
                        if (_curvalue < _max) Materia.SetPropertyValue(progressbar, "Value", _curvalue + 1);
                        Thread.Sleep(1); Application.DoEvents();
                        _curvalue = Materia.GetPropertyValue<int>(progressbar, "Value");
                    }

                    if (Materia.MethodExists(progressbar, "Hide")) Materia.InvokeMethod(progressbar, "Hide");
                }
            }
        }

        #region "WaitToFinish"

        /// <summary>
        /// Synchronizes the specified IAsyncResult object and suspends all preceeding events until it is already finished.
        /// </summary>
        /// <param name="result">IAsyncResult to synchronize</param>
        public static void WaitToFinish(IAsyncResult result)
        { WaitToFinish(result, null); }

        /// <summary>
        /// Synchronizes the specified IAsyncResult object and suspends all preceeding events until it is already finished.
        /// </summary>
        /// <param name="result">IAsyncResult to synchronize</param>
        /// <param name="progressbar">Synchronization progress bar</param>
        public static void WaitToFinish(IAsyncResult result, object progressbar)
        { WaitToFinish(new object[] { result }, progressbar); }

        /// <summary>
        /// Synchronizes the specified Thread object and suspends all preceeding events until it is already finished.
        /// </summary>
        /// <param name="thread">Thread to synchronize</param>
        public static void WaitToFinish(Thread thread)
        { WaitToFinish(thread, null); }

        /// <summary>
        /// Synchronizes the specified Thread object and suspends all preceeding events until it is already finished.
        /// </summary>
        /// <param name="thread">Thread to synchronize</param>
        /// <param name="progressbar">Synchonization progress bar</param>
        public static void WaitToFinish(Thread thread, object progressbar)
        { WaitToFinish(new object[] { thread }, progressbar); }

        /// <summary>
        /// Synchronizes all of the specified sync objects and suspends all preceeding events until all of them are finished.
        /// </summary>
        /// <param name="syncs">Synchronization objects to run</param>
        public static void WaitToFinish(object[] syncs)
        {  WaitToFinish(syncs, null); }

        /// <summary>
        /// Synchronizes all of the specified sync objects and suspends all preceeding events until all of them are finished.
        /// </summary>
        /// <param name="syncs">Synchronization objects to run</param>
        /// <param name="progressbar">Synchronization progress bar</param>
        public static void WaitToFinish(object[] syncs, object progressbar)
        {
            bool _allfinished = (syncs.Length <= 0); bool _finished = false;

            while (!_allfinished)
            {
                _finished = true;

                for (int i = 0; i <= (syncs.Length - 1); i++)
                {
                    object _sync = syncs[i];
                    if (_sync != null)
                    {
                        if (_sync.GetType().Name == typeof(IAsyncResult).Name ||
                            _sync.GetType().Name == typeof(Thread).Name ||
                            _sync.GetType().Name == typeof(AsyncResult).Name) 
                        {
                            if (_sync.GetType().Name == typeof(IAsyncResult).Name) _finished = _finished && ((IAsyncResult)_sync).IsCompleted;
                            else if (_sync.GetType().Name == typeof(AsyncResult).Name) _finished = _finished && ((AsyncResult)_sync).IsCompleted;
                            else _finished = _finished && !((Thread)_sync).IsAlive;
                        }
                    }

                    Thread.Sleep(1); Application.DoEvents();
                }

                if (progressbar != null)
                {
                    int _max = 0;
                    if (Materia.PropertyExists(progressbar, "Maximum")) _max = Materia.GetPropertyValue<int>(progressbar, "Maximum");
                    else
                    {
                        if (Materia.PropertyExists(progressbar, "Max")) _max = Materia.GetPropertyValue<int>(progressbar, "Max");
                        else
                        {
                            if (Materia.PropertyExists(progressbar, "MaxValue")) _max = Materia.GetPropertyValue<int>(progressbar, "MaxValue");
                            else _max = 0;
                        }
                    }

                    if (_max > 0 && Materia.PropertyExists(progressbar, "Value"))
                    {
                        int _curvalue = Materia.GetPropertyValue<int>(progressbar, "Value");
                        if (_curvalue < (_max * 0.95)) Materia.SetPropertyValue(progressbar, "Value", _curvalue + 1);
                    }
                }

                _allfinished = _finished; Thread.Sleep(1); Application.DoEvents();
            }
        }

        #endregion "WaitToFinish"
    }
}
