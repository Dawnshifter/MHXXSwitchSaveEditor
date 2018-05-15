using System.Text;

namespace MHXXSaveEditor.Util
{
    class MaxLengthChecker
    {
        public bool GetMaxLength(string theText, int maxLength)
        {
            int bytes = Encoding.UTF8.GetByteCount(theText);
            if (bytes >= maxLength)
                return true;
            else
                return false;
        }
    }
}
