SepaWriter
===

Manage SEPA (Single Euro Payments Area) CreditTransfer for SEPA or international order.
Only one PaymentInformation is managed but it can manage multiple transactions. The debtor is common to all transactions.


Sample for a quick simple transaction :
```csharp
public class MySepaCreditTransfer
{
  private static SepaCreditTransfer GetSampleCreditTransfert(decimal amount, string bic, string iban, string name, string comment)
  {
      var transfert = new SepaCreditTransfer
      {
          MessageIdentification = "transferId",
          PaymentInfoId = "paymentInfo",
          InitiatingPartyName = "Your name",
          Debtor = new SepaIbanData {Bic = "SOGEFRPPXXX", Iban = "FR7030002005500000157845Z02", Name = "My Corp"} // Your bank data
      };

      transfert.AddCreditTransfer("Transaction#1", null,
                                  new SepaIbanData
                                      {
                                          Bic = bic,
                                          Iban = iban,
                                          Name = name
                                      }, amount, comment);
      return transfert;
  }

  public MySepaCreditTransfer()
  {
    var transfert = GetSampleCreditTransfert(123.45,  "AGRIFRPPXXX", "FR1420041010050500013M02606", "THEIR_NAME", "Payment sample");
    transfert.Save("sample.xml");
  }
}
```

Used library:
---
NUnit 2.6.2 for unit tests
Log4net 2.11 for log (used in XML validator)


License:
---
Copyright 2013 PERRICHOT Florian

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

