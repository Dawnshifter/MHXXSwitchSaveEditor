using System;
using System.Drawing;

namespace MHXXSaveEditor.Util
{
    class ColorBrightness
    {
        // Credits goes to whoever did this tbh, googling this comes up with many result so I don't know who the real person behind it
        public int PerceivedBrightness(Color c)
        {
            return (int)Math.Sqrt(
            c.R * c.R * .241 +
            c.G * c.G * .691 +
            c.B * c.B * .068);
        }
    }
}
