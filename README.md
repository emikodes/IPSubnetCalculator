# IP Subnet Calculator.

### App ***WinForms*** che permette di realizzare il subnetting di una rete, dato un indirizzo IP ad essa appartenente.

## Features:

- Form **ridimensionabile** dinamicamente.
- Lo slider per specificare il numero di sottoreti da realizzare, prevede che il suo valore massimo sia “**dinamico**”, in base alla maschera di sottorete indicata nell’indirizzo IP. ( 2^( /30 - CIDR Dell’IP inserito dall’utente.) )
- Cliccando con il tasto sinistro del mouse sulla TextBox nella quale verranno specificare le sottoreti realizzate, il suo contenuto verrà **copiato nella clipboard**.
- Ogni input è stato verificato e genera un eccezione gestita, al fine di impedire crash inaspettati.
- Il Controllo per l’inserimento di una nuova maschera (Input field con due bottoni + -), rileva automaticamente la maschera dell’indirizzo IP inserito, e non permette di specificare una maschera di valore inferiore.

## Con IP Subnet Calculator puoi:

- Specificare una ***nuova maschera di sottorete da applicare*** all’indirizzo IP indicato, l’applicazione automaticamente ne determinerà la notazione CIDR e visualizzerà la nuova subnet mask in binario.
- Specificare il ***numero di sottoreti da voler realizzare***. (Il Numero di sottoreti prodotte sarà arrotondato alla potenza di 2 più vicina.)

### Note 📌:

Le maschere di sottorete /31 e /32 (255.255.255.254 e 255.255.255.255) sono state omesse in quanto considerate di “local broadcast”, e non prevedono host configurabili.



Copyright (c) 2015, lduchosal
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
