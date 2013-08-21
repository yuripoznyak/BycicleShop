using System.Configuration;
using System.Web.Mvc;

namespace Itransition.Captcha
{
    public class CaptchaValidatorAttribute : ActionFilterAttribute
    {
        private const string _challengeFieldKey = "recaptcha_challenge_field";
        private const string _responseFieldKey = "recaptcha_response_field";

        /// <summary>
        /// Called before the action method is invoked
        /// </summary>
        /// <param name="filterContext">Information about the current request and action</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var captchaChallengeValue = filterContext.HttpContext.Request.Form[_challengeFieldKey];
            var captchaResponseValue = filterContext.HttpContext.Request.Form[_responseFieldKey];
            var captchaValidtor = new Recaptcha.RecaptchaValidator
            {
                PrivateKey = ConfigurationManager.AppSettings["ReCaptchaPrivateKey"],
                RemoteIP = filterContext.HttpContext.Request.UserHostAddress,
                Challenge = captchaChallengeValue,
                Response = captchaResponseValue
            };

            var recaptchaResponse = captchaValidtor.Validate();

            // this will push the result value into a parameter in our Action
            filterContext.ActionParameters["captchaValid"] = recaptchaResponse.IsValid;

            base.OnActionExecuting(filterContext);

            // Add string to Trace for testing
            //filterContext.HttpContext.Trace.Write("Log: OnActionExecuting", String.Format("Calling {0}", filterContext.ActionDescriptor.ActionName));
        }
    }
}
