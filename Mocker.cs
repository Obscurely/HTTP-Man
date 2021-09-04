using Titanium.Web.Proxy.EventArguments;

namespace HTTPMan
{
    public class Mocker
    {
        private void Mock(MockerRule rule, SessionEventArgs e)
        {
            if (rule.Matcher == MockMatcher.ForHost)
            {
                if (Mocker.IsSameHost(rule, e))
                {
                    // . . .
                }
            }
            else if (rule.Matcher == MockMatcher.ForUrl)
            {
                //Mocker.IsSameUrl(rule, e)
            }
            else if (rule.Matcher == MockMatcher.ForUrlsMatchingRegex)
            {
                //Mocker.IsUrlMatchingRegex(rule, e)
            }
            else if (rule.Matcher == MockMatcher.ExactQueryString)
            {
                //Mocker.IsSameQueryString(rule, e)
            }
            else if (rule.Matcher == MockMatcher.IncludingHeaders)
            {
                //Mocker.IsIncludingHeaders(rule, e)
            }
            else if (rule.Matcher == MockMatcher.ExactBody)
            {
                //Mocker.IsSameBody(rule, e)
            }
            else if (rule.Matcher == MockMatcher.BodyIncluding)
            {
                //Mocker.IsBodyIncluding(rule, e)
            }
            else if (rule.Matcher == MockMatcher.ExactJsonBody)
            {
                //Mocker.IsSameJsonBody(rule, e)
            }
            else if (rule.Matcher == MockMatcher.JsonBodyIncluding)
            {
                //Mocker.IsJsonBodyIncluding(rule, e)
            }
        }

        private static bool IsSameHost(MockerRule rule, SessionEventArgs e)
        {
            if (rule.MatcherOptions["host"].Equals(e.HttpClient.Request.Host))
                return true;
            else
                return false;
        }
    }
}