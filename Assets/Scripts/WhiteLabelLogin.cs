using UnityEngine;
using System;
using UnityEngine.UI;
using LootLocker.Requests;

public class WhiteLabelLogin : MonoBehaviour
{
    // Input fields
    [Header("New User")]
    public InputField newUserEmailInputField;
    public InputField newUserPasswordInputField;

    [Header("Existing User")]
    public InputField existingUserEmailInputField;
    public InputField existingUserPasswordInputField;

    public PlayerFilesManager playerFilesManager;

    public CurveAnimatorUI loginCanvas;
    public CurveAnimatorUI filesCanvas;

    // Called when pressing "Log in"
    public void Login()
    {
        string email = existingUserEmailInputField.text;
        string password = existingUserPasswordInputField.text;
        LootLockerSDKManager.WhiteLabelLogin(email, password, false, loginResponse =>
        {
            // Player is logged in, now start a game session
            LootLockerSDKManager.StartWhiteLabelSession((startSessionResponse) =>
            {
                loginCanvas.Hide();
                filesCanvas.Show();
                playerFilesManager.GetFiles();
            });
        });
    }

    // Called when pressing "Create account"
    public void CreateAccount()
    {
        string email = newUserEmailInputField.text;
        string password = newUserPasswordInputField.text;

        LootLockerSDKManager.WhiteLabelSignUp(email, password, (response) =>
        {
        });
    }

    public void ResendVerificationEmail(int playerID)
    {
        // Player ID can be retrieved when starting a session or creating an account.
        // Request a verification email to be sent to the registered user, 
        LootLockerSDKManager.WhiteLabelRequestVerification(playerID, (response) =>
        {
            if(response.success)
            {
                Debug.Log("Verification email sent!");
            }
            else
            {
                Debug.Log("Error sending verification email:" + response.Error);
            }

        });
    }

    public void SendResetPassword()
    {
        // Sends a password reset-link to the email
        LootLockerSDKManager.WhiteLabelRequestPassword("email@email-provider.com", (response) =>
        {
            if(response.success)
            {
                Debug.Log("Password reset link sent!");
            }
            else
            {
                Debug.Log("Error sending password-reset-link:" + response.Error);
            }
        });
    }
}
