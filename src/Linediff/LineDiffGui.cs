using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Linediff
{
    public static class LineDiffGui
    {
        private static List<Diff> diffs = new List<Diff>();

        public static void DisplayLineDiffGui(string file)
        {
            var fileLines = File.ReadAllLines(file);

            string prevLine = null;
            int lineNumber = 1;

            foreach (var line in fileLines)
            {
                var diff = prevLine == null ? new Diff(line) : new Diff(prevLine, line);
                diffs.Add(diff);
                prevLine = line;
                ++lineNumber;
            }

            DoUILoop();
        }

        private static Sdl2Window _window;
        private static GraphicsDevice _gd;
        private static CommandList _cl;
        private static ImGuiRenderer _controller;

        // UI state
        private static Vector3 _clearColor = new Vector3(0.45f, 0.55f, 0.6f);

        static void DoUILoop()
        {
            // Create window, GraphicsDevice, and all resources necessary for the demo.
            VeldridStartup.CreateWindowAndGraphicsDevice(
                new WindowCreateInfo(50, 50, 1280, 720, WindowState.Normal, "ImGui.NET Sample Program"),
                new GraphicsDeviceOptions(true, null, true),
                out _window,
                out _gd);
            _window.Resized += () =>
            {
                _gd.MainSwapchain.Resize((uint)_window.Width, (uint)_window.Height);
                _controller.WindowResized(_window.Width, _window.Height);
            };
            _cl = _gd.ResourceFactory.CreateCommandList();
            _controller = new ImGuiRenderer(_gd, _gd.MainSwapchain.Framebuffer.OutputDescription, _window.Width, _window.Height);

            // Main application loop
            while (_window.Exists)
            {
                InputSnapshot snapshot = _window.PumpEvents();
                if (!_window.Exists) { break; }
                _controller.Update(1f / 60f, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

                SubmitUI();

                _cl.Begin();
                _cl.SetFramebuffer(_gd.MainSwapchain.Framebuffer);
                _cl.ClearColorTarget(0, new RgbaFloat(_clearColor.X, _clearColor.Y, _clearColor.Z, 1f));
                _controller.Render(_gd, _cl);
                _cl.End();
                _gd.SubmitCommands(_cl);
                _gd.SwapBuffers(_gd.MainSwapchain);
            }

            // Clean up Veldrid resources
            _gd.WaitForIdle();
            _controller.Dispose();
            _cl.Dispose();
            _gd.Dispose();
        }

        private static unsafe void SubmitUI()
        {
            {
                ImGui.GetStyle().WindowRounding = 0;

                ImGui.SetNextWindowPos(new Vector2(0, 0));
                ImGui.SetNextWindowSize(new Vector2(_window.Width, _window.Height));

                float statusBarHeight = ImGui.CalcTextSize("1").Y + ImGui.GetStyle().ItemSpacing.Y * 2;

                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f));
                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0.0f));
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0.0f));

                ImGui.Begin("", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration);

                ImGui.BeginChild("Diff", new Vector2(0,-statusBarHeight), true, ImGuiWindowFlags.HorizontalScrollbar);

                ImGui.PopStyleVar(3);

                RenderWithPaging();

                Console.WriteLine($"{ImGui.GetScrollY()}/{ImGui.GetScrollMaxY()}");
                float scrollAreaHeight = ImGui.GetWindowHeight();

                ImGui.End();


                // status bar
                var statusBarRectPos = new Vector2(0, scrollAreaHeight);
                ImGui.GetWindowDrawList().AddRectFilled(statusBarRectPos, statusBarRectPos + new Vector2(ImGui.GetWindowWidth(), scrollAreaHeight), (uint)Color.LightGray.ToArgb());

                ImGui.PushStyleColor(ImGuiCol.Text, (uint)Color.Black.ToArgb());

                ImGui.SetCursorPosX(ImGui.GetStyle().ItemSpacing.X);
                ImGui.SetCursorPosY(statusBarRectPos.Y + ImGui.GetStyle().ItemSpacing.Y);
                ImGui.Text($"lines: {diffs.Count}");

                if (currentLine != -1)
                {
                    ImGui.SameLine();
                    ImGui.Text($"| line {currentLine}");
                }

                if (currentColumn != -1)
                {
                    ImGui.SameLine();
                    ImGui.Text($"| col {currentColumn}");
                }

                ImGui.PopStyleColor(1);

                ImGui.End();
            }
        }

        static float maxScrollY = -1;
        static int currentLine = -1;
        static int currentColumn = -1;

        static bool CanKeyboardNavigate => maxScrollY >= 0.0f;

        static int GetIndex(this ImGuiKey key)
        {
            return ImGui.GetKeyIndex(key);
        }

        static bool IsPressed(this ImGuiKey key)
        {
            return ImGui.IsKeyPressed(key.GetIndex());
        }

        static Vector2 GetScrollPos() => new Vector2(ImGui.GetScrollX(), ImGui.GetScrollY());

        static void RenderWithPaging()
        {
            float lineHeight = ImGui.CalcTextSize("1").Y + ImGui.GetStyle().ItemSpacing.Y;
            float charWidth = ImGui.CalcTextSize(" ").X;
            float pageHeight = ImGui.GetWindowSize().Y;
            int totalElements = diffs.Count;

            float lastScrollY = ImGui.GetScrollY();

            // keyboard navigation
            if (CanKeyboardNavigate)
            {
                if (ImGuiKey.DownArrow.IsPressed())
                {
                    lastScrollY = Math.Min(maxScrollY, lastScrollY + lineHeight);
                    ImGui.SetScrollY(lastScrollY);
                }
                else if (ImGuiKey.UpArrow.IsPressed())
                {
                    lastScrollY = Math.Max(0, lastScrollY - lineHeight);
                    ImGui.SetScrollY(lastScrollY);
                }
                else if (ImGuiKey.PageDown.IsPressed())
                {
                    lastScrollY = Math.Min(maxScrollY, lastScrollY + pageHeight);
                    ImGui.SetScrollY(lastScrollY);
                }
                else if (ImGuiKey.PageUp.IsPressed())
                {
                    lastScrollY = Math.Max(0, lastScrollY - pageHeight);
                    ImGui.SetScrollY(lastScrollY);
                }
                // TODO: handle Ctrl+Home/Ctrl+End for jumping to start/end. Will need to look at how to get this from Veldrid directly, as it doesn't look like Dear ImGui supports this.
            }

            GuiDiffFormatter formatter = new GuiDiffFormatter(GetScrollPos(), lineHeight);

            // calculate first element
            int firstElement = (int)Math.Floor(Math.Max(0, lastScrollY - ImGui.GetWindowSize().Y) / lineHeight);

            float firstElementStart = firstElement * lineHeight;

            ImGui.SetCursorPosY(firstElementStart);

            int elementOffset = 0;

            int lineNumberCharWidth = (diffs.Count + 1).ToString().Length;

            float lineNumberFieldWidth = ImGui.CalcTextSize(new string(' ', lineNumberCharWidth)).X + ImGui.GetStyle().ItemSpacing.X;

            while (firstElementStart + elementOffset * lineHeight - lastScrollY < ImGui.GetWindowSize().Y * 2 && firstElement + elementOffset < totalElements)
            {
                int i = firstElement + elementOffset;

                ImGui.PushStyleColor(ImGuiCol.Text, (uint)Color.DarkGray.ToArgb());
                ImGui.Text((i + 1).ToString().PadLeft(lineNumberCharWidth));
                ImGui.SameLine();
                ImGui.PopStyleColor(1);

                formatter.FormatDiff(diffs[i]);

                ++elementOffset;
            }

            ImGui.SetCursorPosY(totalElements * lineHeight);

            maxScrollY = ImGui.GetScrollMaxY();

            // get mouse cursor pos and set current line and current column

            var mouseWindowPos = ImGui.GetMousePos();
            
            if (mouseWindowPos.Y > ImGui.GetWindowHeight())
            {
                currentLine = -1;
            }
            else
            {
                var posWithinScrollWindow = mouseWindowPos - ImGui.GetWindowPos() + GetScrollPos() + new Vector2(0, ImGui.GetStyle().ItemSpacing.Y / 2.0f);

                currentLine = (int)(posWithinScrollWindow.Y / lineHeight) + 1;

                float lineMinX = lineNumberFieldWidth;
                float lineMaxX = lineMinX + ImGui.CalcTextSize(diffs[currentLine].FullText).X;

                if (posWithinScrollWindow.X >= lineMinX && posWithinScrollWindow.X <= lineMaxX)
                {
                    currentColumn = (int)((posWithinScrollWindow.X - lineMinX) / charWidth) + 1;
                }
                else
                {
                    currentColumn = -1;
                }
            }
        }
    }
}
