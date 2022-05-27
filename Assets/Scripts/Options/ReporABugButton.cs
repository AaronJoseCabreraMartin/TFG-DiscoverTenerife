using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
  * @brief Class that allow open the email app of the user with some information.
  * In order to make easier the report a bug process.
  */
public class ReporABugButton : MonoBehaviour
{
    /**
      * @brief string that stores the developer email.
      */
    [SerializeField] private string email = "alu0101101019@ull.edu.es";

    /**
      * @brief string that stores email's subject.
      */
    [SerializeField] private string subject = "BugReport";

    /**
      * @brief string that stores the email's body.
      */
    [SerializeField] private string body = "Please describe what happened.\nAdding a screenshot of the situation will be helpfully.\n";

    /**
      * @brief This method open the user's default mail aplication with the defined email, subject and body.
      * The subject and the body are formated as URL before.
      */
    public void OpenEmailApp () {
        Application.OpenURL("mailto:" + email + 
                            "?subject=" + formatAsURL(subject) + 
                            "&body=" + formatAsURL(body));
    }

    /**
      * @param string with the url to be formated
      * @brief this method returns the given url but formated
      */
    string formatAsURL (string url) {
        return WWW.EscapeURL(url).Replace("+","%20");
    }
}
