using ModelEntites;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTestImplLibrary
{
    class LetterCodes
    {
        public LetterFunctionResult BeforeLoad(LetterFunctionParam Param)
        {
            LetterFunctionResult result = new LetterFunctionResult();
            Param.Letter.Desc += "BeforeLoad,";
            return result;
        }
        public LetterFunctionResult BeforeSave(LetterFunctionParam Param)
        {
            LetterFunctionResult result = new LetterFunctionResult();
            Param.Letter.Desc += "BeforeSave,";
            return result;
        }
        public LetterFunctionResult AfterSave(LetterFunctionParam Param)
        {
            LetterFunctionResult result = new LetterFunctionResult();
            Param.Letter.Desc += "AfterSave,";
            return result;
        }
        public LetterFunctionResult ExternalCode(LetterFunctionParam Param)
        {
            LetterFunctionResult result = new LetterFunctionResult();
            Param.Letter.Title = " عنوان خارجی";
            Param.Letter.LetterDate = DateTime.Now - TimeSpan.FromDays(1);
            Param.Letter.AttechedFile = new ModelEntites.FileRepositoryDTO();
            var filePath = "D:\\testExternalSource.doc";
            Param.Letter.AttechedFile.FileExtension = System.IO.Path.GetExtension(filePath).Replace(".", "");
                   Param.Letter.AttechedFile.FileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            Param.Letter.AttechedFile.Content = System.IO.File.ReadAllBytes(filePath);
            return result;
        }


        public LetterFunctionResult ConvertToExternal(LetterFunctionParam Param)
        {
            LetterFunctionResult result = new LetterFunctionResult();
            result.ExternalCode = "1111";
            result.Result = true;
            return result;
        }
    }
}
