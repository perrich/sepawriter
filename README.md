SepaWriter
===

Manage SEPA (Single Euro Payments Area) Credit and Debit Transfer for SEPA or international order.
Only one PaymentInformation is managed but it can manage multiple transactions. 
- The debtor is common to all transactions in a Credit transfer.
- The creditor is common to all transactions in a Debit transfer.

It follow "Customer Credit Transfer Initiation" &lt;pain.001.001.03&gt; defined in ISO 20022 but also some specific french rules (field used size != allowed size). Debit uses &lt;pain.008.001.02&gt; defined in ISO 20022 and the same french restrictions.

English usage guide :
http://www.swift.com/assets/corporates/documents/our_solution/implementing_your_project_2009_iso20022_usage_guide.pdf

French usage guide :
http://www.cfonb.org/fichiers/20130612091605_3_15_Guide_ISO20022_remises_informatisees_d_ordres_de_paiement_20100602_virement_V2.0_2010_06.pdf
http://cfonb.inie.makoa.fr/fichiers/20131206162450_2_11_Guide_ISO20022_remises_informatisees_dordres_de_prelevement_SEPA_pain.008__v1.2__2013_12.pdf

Sample
---

Sample for a quick single transaction :
```csharp
public class MySepaCreditTransfer
{
  private static SepaCreditTransfer GetCreditTransfertSample(decimal amount, string bic, string iban,
    string name, string comment)
  {
      var transfert = new SepaCreditTransfer
      {
          MessageIdentification = "uniqueCreditTransfertId",
          InitiatingPartyName = "Your name",
          // Below, your bank data
          Debtor = new SepaIbanData {Bic = "SOGEFRPPXXX", Iban = "FR7030002005500000157845Z02", Name = "My Corp"}          
      };

      transfert.AddCreditTransfer(new SepaCreditTransferTransaction
		{
			Creditor = new SepaIbanData {
			  Bic = bic,
			  Iban = iban,
			  Name = name
			},
			Amount = amount,
			RemittanceInformation = comment
		});
      return transfert;
  }

  public MySepaCreditTransfer()
  {
    var transfert = GetCreditTransfertSample(123.45m,  "AGRIFRPPXXX", "FR1420041010050500013M02606",
      "THEIR_NAME", "Payment sample");
    transfert.Save("sample.xml");
  }
}
```

Used libraries:
---
- NUnit 3.7.1 for unit tests
- Log4net 2.0.8 for log (used in XML validator)


License:
---
Copyright 2013-2017 PERRICHOT Florian

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

