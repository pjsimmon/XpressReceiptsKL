using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using Plugin.Media;
using Plugin.Media.Abstractions;


using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;

namespace XpressReceipt
{
    public class OCR
    {

        const string subscriptionKey = "1c9ce69ee64a4d10998e3683da0d8071";

		const string uriBase = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0";


		public OCR()
        {
        }
    }
}
