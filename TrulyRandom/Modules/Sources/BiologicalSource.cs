using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using TrulyRandom.Models;

namespace TrulyRandom.Modules.Sources;

/// <summary>
/// Recieves data from user actions, i. e. mouse movements and key presses
/// </summary>
public class BiologicalSource : Source
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BiologicalSource" /> class
    /// </summary>
    public BiologicalSource()
    {
        thread = new Thread(new ThreadStart(WorkerThread));
        thread.IsBackground = true;
        thread.Name = Name;
    }

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(ref Point lpPoint);

    [DllImport("user32.dll")]
    private static extern int GetAsyncKeyState(int i);

    private readonly List<byte> intermediateBuffer = new();
    private Point lastPosition = new(int.MinValue, int.MinValue);
    private readonly int[] lastKeyStates = new int[255];

    /// <summary>
    /// Main loop.
    /// </summary>
    private void WorkerThread()
    {
        while (!shouldBeDisposed)
        {
            if (!Run)
            {
                Thread.Sleep(10);
                continue;
            }

            //Cursor position
            Point position = new();

            GetCursorPos(ref position);

            if (position != lastPosition)
            {
                intermediateBuffer.Add((byte)(position.X % 256));
                intermediateBuffer.Add((byte)(position.Y % 256));
                intermediateBuffer.Add((byte)(DateTime.Now.Ticks % 256));
            }
            lastPosition = position;

            //Keystrokes
            for (byte i = 0; i < 255; i++)
            {
                int keyState = GetAsyncKeyState(i);
                if (lastKeyStates[i] != keyState)
                {
                    byte[] bytes = BitConverter.GetBytes(keyState);
                    intermediateBuffer.Add(i);
                    intermediateBuffer.Add((byte)(bytes[0] ^ bytes[1]));
                    intermediateBuffer.Add((byte)(DateTime.Now.Ticks % 256));
                    lastKeyStates[i] = keyState;
                }
            }

            if (intermediateBuffer.Count > 100)
            {
                AddData(intermediateBuffer.ToArray());
                intermediateBuffer.Clear();
            }
        }
    }
}
