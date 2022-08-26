using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography.X509Certificates;


public class BybassHTTPSCertificate : CertificateHandler
{
    // Encoded RSAPublicKey
    private static readonly string PUB_KEY = "";
    
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        //Simply return true no matter what
        // This makes HTTPS not work
        return true;
    }

}
