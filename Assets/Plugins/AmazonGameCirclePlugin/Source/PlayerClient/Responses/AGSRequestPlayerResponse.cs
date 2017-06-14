/**
 * © 2012-2014 Amazon Digital Services, Inc. All rights reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"). You may not use this file except in compliance with the License. A copy
 * of the License is located at
 *
 * http://aws.amazon.com/apache2.0/
 *
 * or in the "license" file accompanying this file. This file is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
 */

using AmazonCommon;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AGSRequestPlayerResponse : AGSRequestResponse {

    public AGSPlayer player;

    public static AGSRequestPlayerResponse FromJSON(string json) {
        try {
            AGSRequestPlayerResponse response = new AGSRequestPlayerResponse ();
            Hashtable hashtable = json.hashtableFromJson();
            response.error = hashtable.ContainsKey("error") ? hashtable ["error"].ToString () : "";
            response.userData = hashtable.ContainsKey("userData") ? int.Parse(hashtable ["userData"].ToString ()) : 0;
            response.player = hashtable.ContainsKey ("player") ? AGSPlayer.fromHashtable (hashtable ["player"] as Hashtable) : AGSPlayer.GetBlankPlayer ();
            return response;
        } catch (Exception e) {
            AGSClient.LogGameCircleError(e.ToString());
            return GetBlankResponseWithError(JSON_PARSE_ERROR);
        }
    }

    public static AGSRequestPlayerResponse GetBlankResponseWithError (string error, int userData = 0) {
        AGSRequestPlayerResponse response = new AGSRequestPlayerResponse ();
        response.error = error;
        response.userData = userData;
        response.player = AGSPlayer.GetBlankPlayer();
        return response;
    }

    public static AGSRequestPlayerResponse GetPlatformNotSupportedResponse (int userData) {
        return GetBlankResponseWithError(PLATFORM_NOT_SUPPORTED_ERROR, userData);
    }

}
