# Offline Crypto Utility
Created by https://twitter.com/kr3at to allow offline Asymmetric Key Exchange using RSA for exchange of AES encrypted messages.

Usage:
You can download and extract the compiled binaries contained in OfflineCrypto.zip and follow the instructions below.

Alternately, you can clone the source from https://github.com/alexhiggins732/OfflineCrypto. The code is a .Net Framework 4.8 Windows Form application is written in C# using Visual Studio 2019. The code should compile in earlier versions.

## Receiving And Decrypting Encrypted Data

**Generate an RSA Key**
![Step 1](docs/step1.png)

 - *Optional: Select an RSA Key Size. By default the most secure option.*
 - 16834 is selected Click Generate.
 - Save the XML in **Private** to a file for later use.
 - Save the XML in **Public** to a file for later use.

**To Receive Encrypted Data**
Give the contents of your **public** key to the person you want to share data with.

**Do not** share your private key.

That person will send you back their AES key with encrypted using your public key and an AES encrypted file protected by their AES key.

**To Decrypt Encrypted Data:**

 - Enter your **Private Key** into **Private** text box the form.
 - Enter the **Encrypted AES Key** into the **AES Base 64** text box.
 - Click the **Decrypt** button in the **AES Key** group box to decrypt the sender's AES key.
 - Browse for the encrypted file sent to you. It will have a ".aes" file extension.
 - Click the **Decrypt** button beneath the file name.
 - A message box will show the location of the decrypted file. It will be in the same location as the encrypted file with the same file name but will no longer have the ".aes" extension

## Sending Encrypted Data
To send encrypted data you will need a **Public** key from the user you wish to give the encrypted data to.

 - Enter the  **Public** key into the  **Public** textbox.
 - Enter a Password to use as an **AES** encryption key in the **AES Key** textbox. It must be at least 32 characters long.
 - Click the Encrypt button in the **AES Key** group box. This will encrypt your **AES Key** using the **Public** key sent to you and populate the **AES Base 64** textbox with the encrypted **AES Key** 
 - Browse for the file you wish to encrypt.
 - Click the **Encrypted** button beneath the file name.
 - A message box will show the location of the encrypted file. It will be in the same location as the file your encrypted. It will have a similar file name with a timestamp appended to it and a ".aes" extension.
 - Send the encrypted file and your encrypted password from **AES Base 64** text box to the person who will be decrypting it. **Do not** share your unecrypted **AES Key**

## The Complete Process
The entire process, as describe in the above two steps, from end to end.

 1. Alice wishes to receive and encrypted file from Bob so Alice first generates an RSA key. 
 2. Alice saves the Private RSA key so she can decrypt the file and AES password Bob will later send her.
 3. Alice sends the Public RSA key to Bob so he can encrypt a file with an AES Password and send Alice the encrypted file and his encrypted AES password.
 4. Bob receives the public RSA key from Alice.
 5. Bob enters the public RSA key into the form.
 6. Bob enters an AES password of at least 32 characters into the form.
 7. Bob encrypts the AES password using the public key Alice sent using the encrypt button in the AES group box.
 8. Bob browses for a file.
 9. Bob clicks encrypt in the file group box.
 10. Bob sends Alice the encrypted file and his encrypted AES Key.
 11. Alice enters her Private RSA key into the form.
 12. Alice enters bobs encrypted AES key into the form.
 13. Alice clicks Decrypt in the AES Key group box to decrypte Bob's AES key.
 14. Alice browses for the encrypted file Bob sent her.
 15. Alice clicks Decrypt in the File group box to decrypt the file. 

## Key Examples
An RSA **Public** key will contain an XML string and have a child Modulus and Exponent element. It will look similar to this:
```markup
<RSAKeyValue>
  <Modulus>va20t6+MzXbQlnzSvq5sOhvWPb5dcvB7tg9kdet9xvCFkBtHdGdevjTQiE1ww0tx1SBO0aGdQRtfrJexR2yvU5HYTq6wg5fG5vAAEOgNikZUUuKobhE9+9bLKIHE3VYJ0LhZm+HEK+nnI4yp5HCGHkKbvG57IvmCU0d10VKlre0=</Modulus>
  <Exponent>AQAB</Exponent>
</RSAKeyValue>
```
An RSA **Private** key will contain an XML string and have a child Modulus, Exponent, P, Q, DP, DQ, InverseQ, and D element. It will look similar to this:
```markup
<RSAKeyValue>
  <Modulus>va20t6+MzXbQlnzSvq5sOhvWPb5dcvB7tg9kdet9xvCFkBtHdGdevjTQiE1ww0tx1SBO0aGdQRtfrJexR2yvU5HYTq6wg5fG5vAAEOgNikZUUuKobhE9+9bLKIHE3VYJ0LhZm+HEK+nnI4yp5HCGHkKbvG57IvmCU0d10VKlre0=</Modulus>
  <Exponent>AQAB</Exponent>
  <P>7e2btsf3Ps7g9HFY1nJI89v032tmLhpnyVp0/xexNGluebbupiIEHam0/gFBYmUI44C0MWyCNh2lFAcRECyvKw==</P>
  <Q>zBXojwXGF6q9qe9LneAs0bPZiC7+WvbwVwyVyvImTI06/k+ZOVf1ufwwBgRo27JTzgndqC8y3Hi9uKZQ+Q7LRw==</Q>
  <DP>yr0BRoq2H4rhHOnIrVLM3x66VfaKJxbRAT5XG1bw5JxCSuyaBm5N24jUdOxU7qbxIAp3gPXZLousMpii7Yll6Q==</DP>
  <DQ>ANmig14Byj1y1s9hYEH7zc/S+yc+9gALVkF+Kj9B+5WnBkIsoDmGk4TCanQAP9IJWVVfMAEqVBLIr4k50x/Fpw==</DQ>
  <InverseQ>fflpgDF2IzU1TO5LjHAzo8H45HsMNXJ1WHYYfYyxF2g/+fJULtK4rusHaiSpOwy8+L5jyu9Z3mKDi4vyCpCYaw==</InverseQ>
  <D>R4NngF3Co3CpgiN7vYK7sUjvhIXya7R5oBG7ma341Pm4EbYHQb77fJEjElDINAmM2IL+1JCvXm24q7ThlQFINH5AbzQOVBsozRxhFB2qKjfGYACZMUTTN8I7iuzLqY5FU2YwLHoUiT5StQYFjR+YnIhaRYitmJ0Y1aklSrgOP20=</D>
</RSAKeyValue>
```
An AES Key is composed of plain text and can be anything that is at least 32 characters. For example:

    tH!2 is s0m3 v3RY 53cUR3 v3rY lOng pa22WOrD tO pRo73Ct MY f1L3s

Your AES Base64 key with contain a Base 64 string containing the RSA encrypted AES Key. For example:

    rlQnRMSSjNKklVBQK/1pUBVd3vV03OGm+ZCn8PKhFV27fVfLc7kNKb0PAbnlW9HIBK0QolaDMe1Jrb+515dsah40hwuSc/HQiMjBrFEYFhwR13zZ9n9MpUAGTg81W47b44CtRZGKGhRBQmtMTOu4SJBnfQoPrTLd5hBYh8cicFs=

## Roadmap

If the community finds this tool useful future enhancements may be:
 - RSA encrypt the salt in the header of the AES files.
 - Add text boxes for encrypting and decrypting plain text
 - Implement additional algorithms such as PGP
 - Implement different Key Exchange algorithms such as Diffie Hellman.
 - Ability save and load keys from files, including JSON, XML, PEM, PFK and CRT files.
 - Networking Capabilities for Online Data Exchange and Messaging. - 

