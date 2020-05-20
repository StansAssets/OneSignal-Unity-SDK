/**
 * Modified MIT License
 *
 * Copyright 2016 OneSignal
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * 1. The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * 2. All copies of substantial portions of the Software may only be used in connection
 * with services provided by OneSignal.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using System.Collections.Generic;

using OneSignalPush.MiniJSON;
using System;
using System.Text;
using OneSignalPush;
using UnityEngine.Serialization;

public class GameControllerExample : MonoBehaviour {

    static string s_ExtraMessage;
    [FormerlySerializedAs("email")]
    public string Email = "Email Address";
    [FormerlySerializedAs("externalId")]
    public string ExternalId = "External User ID";

    static readonly bool s_RequiresUserPrivacyConsent = false;

    const float k_ButtonHeight = 35.0f;
    const float k_Spacing = 2.0f;
    const float k_MinorPadding = 2.0f;
    const float k_Padding = 6.0f;

    bool m_VibrateState = true;
    bool m_SoundState = true;
    bool m_SubscriptionState = true;
    bool m_LocationSharedState = true;

    void Start() {
        s_ExtraMessage = null;

        // Enable line below to debug issues with setting up OneSignal. (logLevel, visualLogLevel)
        OneSignal.SetLogLevel(OneSignal.LOG_LEVEL.VERBOSE, OneSignal.LOG_LEVEL.NONE);

        // If you set to true, the user will have to provide consent
        // using OneSignal.UserDidProvideConsent(true) before the
        // SDK will initialize
        OneSignal.SetRequiresUserPrivacyConsent(s_RequiresUserPrivacyConsent);

        // The only required method you need to call to setup OneSignal to receive push notifications.
        // Call before using any other methods on OneSignal (except setLogLevel or SetRequiredUserPrivacyConsent)
        // Should only be called once when your app is loaded.
        // OneSignal.Init(OneSignal_AppId);
        OneSignal.StartInit("99015f5e-87b1-462e-a75b-f99bf7c2822e")
            .HandleNotificationReceived(HandleNotificationReceived)
            .HandleNotificationOpened(HandleNotificationOpened)
            .HandleInAppMessageClicked(HandlerInAppMessageClicked)
            .EndInit();

        OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;
        OneSignal.permissionObserver += OneSignal_permissionObserver;
        OneSignal.subscriptionObserver += OneSignal_subscriptionObserver;
        OneSignal.emailSubscriptionObserver += OneSignal_emailSubscriptionObserver;

        OneSignal.GetPermissionSubscriptionState();
    }


    // Examples of using OneSignal External User Id
    void OneSignalExternalUserIdCallback(Dictionary<string, object> results)
    {
        // The results will contain push and email success statuses
        Console.WriteLine("External user id updated with results: " + Json.Serialize(results));

        // Push can be expected in almost every situation with a success status, but
        // as a pre-caution its good to verify it exists
        if (results.ContainsKey("push"))
        {
            if (results["push"] is Dictionary<string, object> pushStatusDict && pushStatusDict.ContainsKey("success"))
            {
                Console.WriteLine("External user id updated for push with results: " + pushStatusDict["success"]);
            }
        }

        // Verify the email is set or check that the results have an email success status
        if (results.ContainsKey("email"))
        {
            if (results["email"] is Dictionary<string, object> emailStatusDict && emailStatusDict.ContainsKey("success"))
            {
                Console.WriteLine("External user id updated for email with results: " + emailStatusDict["success"]);
            }
        }
    }

   void PrintOutcomeEvent(OSOutcomeEvent outcomeEvent) {
        Console.WriteLine(outcomeEvent.session + "\n" +
                string.Join(", ", outcomeEvent.notificationIds) + "\n" +
                outcomeEvent.name + "\n" +
                outcomeEvent.timestamp + "\n" +
                outcomeEvent.weight);
    }

   void OneSignal_subscriptionObserver(OSSubscriptionStateChanges stateChanges) {
	    Debug.Log("SUBSCRIPTION stateChanges: " + stateChanges);
	    Debug.Log("SUBSCRIPTION stateChanges.to.userId: " + stateChanges.to.userId);
	    Debug.Log("SUBSCRIPTION stateChanges.to.subscribed: " + stateChanges.to.subscribed);
   }

   void OneSignal_permissionObserver(OSPermissionStateChanges stateChanges) {
	    Debug.Log("PERMISSION stateChanges.from.status: " + stateChanges.from.status);
	    Debug.Log("PERMISSION stateChanges.to.status: " + stateChanges.to.status);
   }

   void OneSignal_emailSubscriptionObserver(OSEmailSubscriptionStateChanges stateChanges) {
	    Debug.Log("EMAIL stateChanges.from.status: " + stateChanges.from.emailUserId + ", " + stateChanges.from.emailAddress);
	    Debug.Log("EMAIL stateChanges.to.status: " + stateChanges.to.emailUserId + ", " + stateChanges.to.emailAddress);
    }

    // Called when your app is in focus and a notification is received.
    // The name of the method can be anything as long as the signature matches.
    // Method must be static or this object should be marked as DontDestroyOnLoad
    static void HandleNotificationReceived(OSNotification notification) {
        OSNotificationPayload payload = notification.payload;
        string message = payload.body;

        print("GameControllerExample:HandleNotificationReceived: " + message);
        print("displayType: " + notification.displayType);
        s_ExtraMessage = "Notification received with text: " + message;

        Dictionary<string, object> additionalData = payload.additionalData;
        if (additionalData == null)
            Debug.Log ("[HandleNotificationReceived] Additional Data == null");
        else
            Debug.Log("[HandleNotificationReceived] message " + message + ", additionalData: " + Json.Serialize(additionalData));
    }

    // Called when a notification is opened.
    // The name of the method can be anything as long as the signature matches.
    // Method must be static or this object should be marked as DontDestroyOnLoad
    public static void HandleNotificationOpened(OSNotificationOpenedResult result) {
        OSNotificationPayload payload = result.notification.payload;
        string message = payload.body;
        string actionId = result.action.actionID;

        print("GameControllerExample:HandleNotificationOpened: " + message);
        s_ExtraMessage = "Notification opened with text: " + message;

        Dictionary<string, object> additionalData = payload.additionalData;
        if (additionalData == null)
            Debug.Log ("[HandleNotificationOpened] Additional Data == null");
        else
            Debug.Log("[HandleNotificationOpened] message " + message + ", additionalData: " + Json.Serialize(additionalData));

        if (actionId != null) {
            // actionSelected equals the id on the button the user pressed.
            // actionSelected will equal "__DEFAULT__" when the notification itself was tapped when buttons were present.
            s_ExtraMessage = "Pressed ButtonId: " + actionId;
        }
    }

    public static void HandlerInAppMessageClicked(OSInAppMessageAction action) {
        String logInAppClickEvent = "In-App Message Clicked: " +
            "\nClick Name: " + action.clickName +
            "\nClick Url: " + action.clickUrl +
            "\nFirst Click: " + action.firstClick +
            "\nCloses Message: " + action.closesMessage;

        print(logInAppClickEvent);
        s_ExtraMessage = logInAppClickEvent;
    }

    void OnGUI()
    {
        float panelWidth = Screen.width / 2.0f;
        Rect rect = new Rect(0.0f, 0.0f, panelWidth, k_ButtonHeight);
        GUI.Label(rect.WithWidth(Screen.width), "One Signal Example", GUIStylesProvider.BoldLabel);
        rect = rect.BelowSelf();

        rect = NotificationsGUI("Notifications", rect);
        TaggingGUI("Tagging", rect.RightOfSelf());

        rect = rect.BelowSelf().Translate(0.0f, k_Padding);
        rect = InAppMessagingGUI("In-App Messaging", rect);
        OutcomesGUI("Outcomes", rect.RightOfSelf());

        rect = rect.BelowSelf().Translate(0.0f, k_Padding);
        rect = EmailGUI("Email", rect);
        AppearanceGUI("Appearance", rect.RightOfSelf());

        rect = rect.BelowSelf().Translate(0.0f, k_Padding);
        rect = UserStatusGUI("User Status", rect);
        ExternalUserIDsGUI("External User IDs", rect.RightOfSelf());

        rect = rect.BelowSelf().Translate(0.0f, k_Padding);
        rect = LocationDataGUI("Location Data", rect);
        PrivacyGUI("Privacy", rect.RightOfSelf());

        rect = rect.BelowSelf().WithHeight(k_ButtonHeight).Translate(0.0f, k_Padding);
        GUI.Label(rect.PadHorizontally(k_Padding), "Output:", GUIStylesProvider.BottomLeftLabel);
        rect = rect.BelowSelf().WithSize(Screen.width, k_ButtonHeight * 3.0f).PadHorizontally(k_MinorPadding);
        GUI.Box(rect, GUIContent.none);
        GUI.Label(rect.PadSides(k_MinorPadding), s_ExtraMessage, GUIStylesProvider.WrappedLabel);
    }

    Rect NotificationsGUI(string title, Rect rect)
    {
        const float sectionHeight = 220.0f;

        Rect currentRect = rect.WithHeight(sectionHeight).PadHorizontally(k_MinorPadding);
        GUI.Box(rect.WithHeight(sectionHeight).PadHorizontally(k_MinorPadding), GUIContent.none);

        float height = 0.0f;
        currentRect = currentRect.WithSize(currentRect.width, k_ButtonHeight).PadHorizontally(k_MinorPadding);
        GUI.Label(currentRect, title, GUIStylesProvider.BoldLabel);
        height += currentRect.height;

        currentRect = currentRect.BelowSelf();
        if (GUI.Button(currentRect, "Get Ids", GUIStylesProvider.ButtonLabel))
        {
            OneSignal.IdsAvailable((userId, pushToken) => {
                s_ExtraMessage = "UserID:\n" + userId + "\n\nPushToken:\n" + pushToken;
            });
        }
        height += currentRect.height + k_Spacing;

        currentRect = currentRect.BelowSelf().Translate(0.0f, k_MinorPadding);
        if (GUI.Button(currentRect, "Post Notification", GUIStylesProvider.ButtonLabel))
        {
            s_ExtraMessage = "Waiting to get a OneSignal userId. Uncomment OneSignal.SetLogLevel in the Start method if it hangs here to debug the issue.";
            OneSignal.IdsAvailable((userId, pushToken) => {
                if (pushToken != null) {
                    // See http://documentation.onesignal.com/docs/notifications-create-notification for a full list of options.
                    // You can not use included_segments or any fields that require your OneSignal 'REST API Key' in your app for security reasons.
                    // If you need to use your OneSignal 'REST API Key' you will need your own server where you can make this call.

                    var notification = new Dictionary<string, object>();
                    notification["contents"] = new Dictionary<string, string>() { {"en", "Test Message"} };
                    // Send notification to this device.
                    notification["include_player_ids"] = new List<string>() { userId };
                    // Example of scheduling a notification in the future.
                    //notification["send_after"] = System.DateTime.Now.ToUniversalTime().AddSeconds(30).ToString("U");

                    s_ExtraMessage = "Posting test notification now.";

                    OneSignal.PostNotification(notification, (responseSuccess) => {
                        s_ExtraMessage = "Notification posted successful! Delayed by about 30 seconds to give you time to press the home button to see a notification vs an in-app alert.\n" + Json.Serialize(responseSuccess);
                    }, (responseFailure) => {
                        s_ExtraMessage = "Notification failed to post:\n" + Json.Serialize(responseFailure);
                    });
                } else {
                    s_ExtraMessage = "ERROR: Device is not registered.";
                }
            });
        }
        height += currentRect.height + k_Spacing;

        currentRect = currentRect.BelowSelf().Translate(0.0f, k_MinorPadding);
        if (GUI.Button(currentRect, "Clear Notifications", GUIStylesProvider.ButtonLabel))
        {
            OneSignal.ClearOneSignalNotifications();
        }
        height += currentRect.height + k_Spacing;

        currentRect = currentRect.BelowSelf().Translate(0.0f, k_MinorPadding);
        if (GUI.Button(currentRect, "Register For Notifications", GUIStylesProvider.ButtonLabel))
        {
            OneSignal.RegisterForPushNotifications();
        }
        height += currentRect.height + k_Spacing;

        currentRect = currentRect.BelowSelf().Translate(0.0f, k_MinorPadding);
        if (GUI.Button(currentRect, "Prompt For Notifications", GUIStylesProvider.ButtonLabel))
        {
            OneSignal.PromptForPushNotificationsWithUserResponse((accepted) =>
            {
                s_ExtraMessage = $"Prompt For Notifications With User Response. Accepted: {accepted}";
            });
        }
        height += currentRect.height + k_Spacing;

        return rect.WithHeight(height);
    }

    Rect TaggingGUI(string title, Rect rect)
    {
        const float sectionHeight = 146.0f;

        Rect currentRect = rect.WithHeight(sectionHeight).PadHorizontally(k_MinorPadding);
        GUI.Box(rect.WithHeight(sectionHeight).PadHorizontally(k_MinorPadding), GUIContent.none);

        float height = 0.0f;
        currentRect = currentRect.WithSize(currentRect.width, k_ButtonHeight).PadHorizontally(k_MinorPadding);
        GUI.Label(currentRect, title, GUIStylesProvider.BoldLabel);
        height += currentRect.height;

        currentRect = currentRect.BelowSelf();
        if (GUI.Button(currentRect, "Send Tags", GUIStylesProvider.ButtonLabel))
        {
            // You can tags users with key value pairs like this:
            OneSignal.SendTag("UnityTestKey", "TestValue");
            // Or use an IDictionary if you need to set more than one tag.
            OneSignal.SendTags(new Dictionary<string, string>() { { "UnityTestKey2", "value2" }, { "UnityTestKey3", "value3" } });

            // You can delete a single tag with it's key.
            // OneSignal.DeleteTag("UnityTestKey");
            // Or delete many with an IList.
            // OneSignal.DeleteTags(new List<string>() {"UnityTestKey2", "UnityTestKey3" });
        }
        height += currentRect.height + k_Spacing;

        currentRect = currentRect.BelowSelf().Translate(0.0f, k_MinorPadding);
        if (GUI.Button(currentRect, "Get Tags", GUIStylesProvider.ButtonLabel))
        {
            OneSignal.GetTags(tags =>
            {
                StringBuilder builder = new StringBuilder("Tags: ");
                foreach (var tagPair in tags)
                {
                    builder.Append($"{tagPair.Key}:{tagPair.Value} ");
                }
                s_ExtraMessage = builder.ToString();
            });
        }
        height += currentRect.height + k_Spacing;

        currentRect = currentRect.BelowSelf().Translate(0.0f, k_MinorPadding);
        if (GUI.Button(currentRect, "Delete Tags", GUIStylesProvider.ButtonLabel))
        {
            OneSignal.DeleteTags(new List<string> {"UnityTestKey", "UnityTestKey2", "UnityTestKey3"});
        }
        height += currentRect.height + k_Spacing;

        return rect.WithHeight(height);
    }

    Rect InAppMessagingGUI(string title, Rect rect)
    {
        const float sectionHeight = 183.0f;

        Rect currentRect = rect.WithHeight(sectionHeight).PadHorizontally(k_MinorPadding);
        GUI.Box(rect.WithHeight(sectionHeight).PadHorizontally(k_MinorPadding), GUIContent.none);

        float height = 0.0f;
        currentRect = currentRect.WithSize(currentRect.width, k_ButtonHeight).PadHorizontally(k_MinorPadding);
        GUI.Label(currentRect, title, GUIStylesProvider.BoldLabel);
        height += currentRect.height;

        currentRect = currentRect.BelowSelf();
        if (GUI.Button(currentRect, "Add Trigger", GUIStylesProvider.ButtonLabel))
        {
            // Add a single trigger
            OneSignal.AddTrigger("key", "value");

            // Get the current value to a trigger by key
            var triggerKey = "key";
            var triggerValue = OneSignal.GetTriggerValueForKey(triggerKey);
            String output = "Trigger key: " + triggerKey + " value: " + (String) triggerValue;
            Console.WriteLine(output);

            // Add multiple triggers
            OneSignal.AddTriggers(new Dictionary<string, object> { { "key1", "value1" }, { "key2", 2 } });
        }
        height += currentRect.height + k_Spacing;

        currentRect = currentRect.BelowSelf().Translate(0.0f, k_MinorPadding);
        if (GUI.Button(currentRect, "Remove Triggers", GUIStylesProvider.ButtonLabel))
        {
            // Delete a trigger
            OneSignal.RemoveTriggerForKey("key");

            // Delete a list of triggers
            OneSignal.RemoveTriggersForKeys(new List<string>() { "key1", "key2" });
        }
        height += currentRect.height + k_Spacing;

        currentRect = currentRect.BelowSelf().Translate(0.0f, k_MinorPadding);
        if (GUI.Button(currentRect, "Get Trigger Value", GUIStylesProvider.ButtonLabel))
        {
            s_ExtraMessage = $"Trigger Value: {OneSignal.GetTriggerValueForKey("key1")}";
        }
        height += currentRect.height + k_Spacing;

        currentRect = currentRect.BelowSelf().Translate(0.0f, k_MinorPadding);
        if (GUI.Button(currentRect, "Pause In-App Messages", GUIStylesProvider.ButtonLabel))
        {
            // Temporarily pause In-App messages; If true is passed in.
            // Great to ensure you never interrupt your user while they are in the middle of a match in your game.
            OneSignal.PauseInAppMessages(false);
        }
        height += currentRect.height + k_Spacing;

        return rect.WithHeight(height);
    }

    Rect OutcomesGUI(string title, Rect rect)
    {
        const float sectionHeight = 146.0f;

        Rect currentRect = rect.WithHeight(sectionHeight).PadHorizontally(k_MinorPadding);
        GUI.Box(rect.WithHeight(sectionHeight).PadHorizontally(k_MinorPadding), GUIContent.none);

        float height = 0.0f;
        currentRect = currentRect.WithSize(currentRect.width, k_ButtonHeight).PadHorizontally(k_MinorPadding);
        GUI.Label(currentRect, title, GUIStylesProvider.BoldLabel);
        height += currentRect.height;

        currentRect = currentRect.BelowSelf();
        if (GUI.Button(currentRect, "Send Outcome", GUIStylesProvider.ButtonLabel))
        {
            OneSignal.SendOutcome("normal_1");
            OneSignal.SendOutcome("normal_2", PrintOutcomeEvent);
        }
        height += currentRect.height + k_Spacing;

        currentRect = currentRect.BelowSelf().Translate(0.0f, k_MinorPadding);
        if (GUI.Button(currentRect, "Send Unique Outcome", GUIStylesProvider.ButtonLabel))
        {
            OneSignal.SendUniqueOutcome("unique_1");
            OneSignal.SendUniqueOutcome("unique_2", PrintOutcomeEvent);
        }
        height += currentRect.height + k_Spacing;

        currentRect = currentRect.BelowSelf().Translate(0.0f, k_MinorPadding);
        if (GUI.Button(currentRect, "Send Outcome With Value", GUIStylesProvider.ButtonLabel))
        {
            OneSignal.SendOutcomeWithValue("value_1", 3.2f);
            OneSignal.SendOutcomeWithValue("value_2", 3.2f, PrintOutcomeEvent);
        }
        height += currentRect.height + k_Spacing;

        return rect.WithHeight(height);
    }

    Rect EmailGUI(string title, Rect rect)
    {
        const float sectionHeight = 109.0f;

        Rect currentRect = rect.WithHeight(sectionHeight).PadHorizontally(k_MinorPadding);
        GUI.Box(rect.WithHeight(sectionHeight).PadHorizontally(k_MinorPadding), GUIContent.none);

        float height = 0.0f;
        currentRect = currentRect.WithSize(currentRect.width, k_ButtonHeight).PadHorizontally(k_MinorPadding);
        GUI.Label(currentRect, title, GUIStylesProvider.BoldLabel);
        height += currentRect.height;

        currentRect = currentRect.BelowSelf();
        if (GUI.Button(currentRect, "Set Email", GUIStylesProvider.ButtonLabel))
        {
            s_ExtraMessage = "Setting email to " + Email;

            OneSignal.SetEmail (Email, () => {
                Debug.Log("Successfully set email");
            }, (error) => {
                Debug.Log("Encountered error setting email: " + Json.Serialize(error));
            });
        }
        height += currentRect.height + k_Spacing;

        currentRect = currentRect.BelowSelf().Translate(0.0f, k_MinorPadding);
        if (GUI.Button(currentRect, "Logout Email", GUIStylesProvider.ButtonLabel))
        {
            s_ExtraMessage = "Logging Out of example@example.com";

            OneSignal.LogoutEmail (() => {
                Debug.Log("Successfully logged out of email");
            }, (error) => {
                Debug.Log("Encountered error logging out of email: " + Json.Serialize(error));
            });
        }
        height += currentRect.height + k_Spacing;

        return rect.WithHeight(height);
    }

    Rect AppearanceGUI(string title, Rect rect)
    {
        const float sectionHeight = 109.0f;

        Rect currentRect = rect.WithHeight(sectionHeight).PadHorizontally(k_MinorPadding);
        GUI.Box(rect.WithHeight(sectionHeight).PadHorizontally(k_MinorPadding), GUIContent.none);

        float height = 0.0f;
        currentRect = currentRect.WithSize(currentRect.width, k_ButtonHeight).PadHorizontally(k_MinorPadding);
        GUI.Label(currentRect, title, GUIStylesProvider.BoldLabel);
        height += currentRect.height;

        currentRect = currentRect.BelowSelf();
        if (GUI.Button(currentRect, "Enable Vibrate", GUIStylesProvider.ButtonLabel))
        {
            m_VibrateState = !m_VibrateState;
            OneSignal.EnableVibrate(m_VibrateState);

            s_ExtraMessage = $"Vibration set to:{m_VibrateState}";
        }
        height += currentRect.height + k_Spacing;

        currentRect = currentRect.BelowSelf().Translate(0.0f, k_MinorPadding);
        if (GUI.Button(currentRect, "Enable Sound", GUIStylesProvider.ButtonLabel))
        {
            m_SoundState = !m_SoundState;
            OneSignal.EnableSound(m_SoundState);

            s_ExtraMessage = $"Sound set to:{m_SoundState}";
        }
        height += currentRect.height + k_Spacing;

        return rect.WithHeight(height);
    }

    Rect UserStatusGUI(string title, Rect rect)
    {
        const float sectionHeight = 109.0f;

        Rect currentRect = rect.WithHeight(sectionHeight).PadHorizontally(k_MinorPadding);
        GUI.Box(rect.WithHeight(sectionHeight).PadHorizontally(k_MinorPadding), GUIContent.none);

        float height = 0.0f;
        currentRect = currentRect.WithSize(currentRect.width, k_ButtonHeight).PadHorizontally(k_MinorPadding);
        GUI.Label(currentRect, title, GUIStylesProvider.BoldLabel);
        height += currentRect.height;

        currentRect = currentRect.BelowSelf();
        if (GUI.Button(currentRect, "Set Subscription", GUIStylesProvider.ButtonLabel))
        {
            m_SubscriptionState = !m_SubscriptionState;
            OneSignal.SetSubscription(m_SubscriptionState);

            s_ExtraMessage = $"Subscription set to:{m_SubscriptionState}";
        }
        height += currentRect.height + k_Spacing;

        currentRect = currentRect.BelowSelf().Translate(0.0f, k_MinorPadding);
        if (GUI.Button(currentRect, "Get Subscription State", GUIStylesProvider.ButtonLabel))
        {
            OneSignal.GetPermissionSubscriptionState();
        }
        height += currentRect.height + k_Spacing;

        return rect.WithHeight(height);
    }

    Rect ExternalUserIDsGUI(string title, Rect rect)
    {
        const float sectionHeight = 109.0f;

        Rect currentRect = rect.WithHeight(sectionHeight).PadHorizontally(k_MinorPadding);
        GUI.Box(rect.WithHeight(sectionHeight).PadHorizontally(k_MinorPadding), GUIContent.none);

        float height = 0.0f;
        currentRect = currentRect.WithSize(currentRect.width, k_ButtonHeight).PadHorizontally(k_MinorPadding);
        GUI.Label(currentRect, title, GUIStylesProvider.BoldLabel);
        height += currentRect.height;

        currentRect = currentRect.BelowSelf();
        if (GUI.Button(currentRect, "Set External User Id", GUIStylesProvider.ButtonLabel))
        {
            OneSignal.SetExternalUserId(ExternalId, OneSignalExternalUserIdCallback);
        }
        height += currentRect.height + k_Spacing;

        currentRect = currentRect.BelowSelf().Translate(0.0f, k_MinorPadding);
        if (GUI.Button(currentRect, "Remove External User Id", GUIStylesProvider.ButtonLabel))
        {
            OneSignal.RemoveExternalUserId(OneSignalExternalUserIdCallback);
        }
        height += currentRect.height + k_Spacing;

        return rect.WithHeight(height);
    }

    Rect LocationDataGUI(string title, Rect rect)
    {
        const float sectionHeight = 109.0f;

        Rect currentRect = rect.WithHeight(sectionHeight).PadHorizontally(k_MinorPadding);
        GUI.Box(rect.WithHeight(sectionHeight).PadHorizontally(k_MinorPadding), GUIContent.none);

        float height = 0.0f;
        currentRect = currentRect.WithSize(currentRect.width, k_ButtonHeight).PadHorizontally(k_MinorPadding);
        GUI.Label(currentRect, title, GUIStylesProvider.BoldLabel);
        height += currentRect.height;

        currentRect = currentRect.BelowSelf();
        if (GUI.Button(currentRect, "Set Location Shared", GUIStylesProvider.ButtonLabel))
        {
            OneSignal.SetLocationShared(m_LocationSharedState);

            s_ExtraMessage = $"Location Shared set to:{m_LocationSharedState}";
        }
        height += currentRect.height + k_Spacing;

        currentRect = currentRect.BelowSelf().Translate(0.0f, k_MinorPadding);
        if (GUI.Button(currentRect, "Prompt Location", GUIStylesProvider.ButtonLabel))
        {
            OneSignal.PromptLocation();
        }
        height += currentRect.height + k_Spacing;

        return rect.WithHeight(height);
    }

    Rect PrivacyGUI(string title, Rect rect)
    {
        const float sectionHeight = 109.0f;

        Rect currentRect = rect.WithHeight(sectionHeight).PadHorizontally(k_MinorPadding);
        GUI.Box(rect.WithHeight(sectionHeight).PadHorizontally(k_MinorPadding), GUIContent.none);

        float height = 0.0f;
        currentRect = currentRect.WithSize(currentRect.width, k_ButtonHeight).PadHorizontally(k_MinorPadding);
        GUI.Label(currentRect, title, GUIStylesProvider.BoldLabel);
        height += currentRect.height;

        currentRect = currentRect.BelowSelf();
        string buttonLabel = OneSignal.UserProvidedConsent() ? "Revoke Privacy Consent" : "Provide Privacy Consent";
        if (GUI.Button(currentRect, buttonLabel, GUIStylesProvider.ButtonLabel))
        {
            s_ExtraMessage = "Providing user privacy consent";

            OneSignal.UserDidProvideConsent(!OneSignal.UserProvidedConsent());
        }
        height += currentRect.height + k_Spacing;

        currentRect = currentRect.BelowSelf().Translate(0.0f, k_MinorPadding);
        if (GUI.Button(currentRect, "Set Requires User Privacy Consent", GUIStylesProvider.ButtonLabel))
        {
            // If you set to true, the user will have to provide consent
            // using OneSignal.UserDidProvideConsent(true) before the
            // SDK will initialize
            OneSignal.SetRequiresUserPrivacyConsent(s_RequiresUserPrivacyConsent);
        }
        height += currentRect.height + k_Spacing;

        return rect.WithHeight(height);
    }
}
