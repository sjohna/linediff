using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;
using ImGuiNET;

namespace Linediff
{
    class GuiDiffFormatter
    {
        float lastScrollY;
        float lineHeight;

        public GuiDiffFormatter(float lastScrollY, float lineHeight)
        {
            this.lastScrollY = lastScrollY;
            this.lineHeight = lineHeight;
        }

        public void FormatDiff(Diff diff)
        {
            float totalWidth = 0.0f;

            for (int i = 0; i < diff.Blocks.Count; ++i)
            {
                totalWidth += Render(diff.Blocks[i] as dynamic);
                if (i != diff.Blocks.Count - 1) ImGui.SameLine(totalWidth);
            }
        }

        private float Render(TextBlock block)
        {
            var textWidth = ImGui.CalcTextSize(block.Text).X;

            ImGui.Text(block.Text);

            return textWidth;
        }

        private float Render(UnchangedTextBlock block)
        {
            var textWidth = ImGui.CalcTextSize(block.Text).X;

            ImGui.Text(block.Text);

            return textWidth;
        }

        private float Render(ChangedTextBlock block)
        {
            var rectPos = ImGui.GetCursorPos() + ImGui.GetWindowPos() - new Vector2(0, lastScrollY);// - ImGui.GetStyle().ItemSpacing / 2.0f;
            var textWidth = ImGui.CalcTextSize(block.Text).X;

            ImGui.GetWindowDrawList().AddRectFilled(rectPos, rectPos + new Vector2(textWidth, lineHeight), (uint)Color.Magenta.ToArgb());

            ImGui.Text(block.Text);

            return textWidth;
        }

        private float Render(AddedTextBlock block)
        {
            var rectPos = ImGui.GetCursorPos() + ImGui.GetWindowPos() - new Vector2(0, lastScrollY) - ImGui.GetStyle().ItemSpacing / 2.0f;
            var textWidth = ImGui.CalcTextSize(block.Text).X;

            ImGui.GetWindowDrawList().AddRectFilled(rectPos, rectPos + new Vector2(textWidth, lineHeight), (uint)Color.Green.ToArgb());

            ImGui.Text(block.Text);

            return textWidth;
        }

        private float Render(RemovedTextBlock block)
        {
            var rectPos = ImGui.GetCursorPos() + ImGui.GetWindowPos() - new Vector2(0, lastScrollY) - ImGui.GetStyle().ItemSpacing / 2.0f;
            var textWidth = ImGui.CalcTextSize(block.Text).X;

            ImGui.GetWindowDrawList().AddRectFilled(rectPos, rectPos + new Vector2(textWidth, lineHeight), (uint)Color.Red.ToArgb());

            ImGui.Text(block.Text);

            return textWidth;
        }
    }
}
