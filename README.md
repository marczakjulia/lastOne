In here Im trying to explain the best what I did and how it works so you figure out how to test it
1. Registering
    1. I didn’t include authorisation here for now so you can easily create an account for yourself. But bear in mind that I know that it should be for admin use only and its just done for the simplicity of checking my work
    2. I am using tokens, so remember to change your account to admin to access all the endpoints (role id = 1)
2. Clients
    1. Clients are divided into individuals or companies
        1. This can be seen cause we have different endpoints for them
        2. There is checking of uniqueness and if someone wants to alter the immutable fields, it just doesn’t occur (referring to pesel and krs)
    2. For the soft delete I just inserted “delete” everywhere so the record of it being here still exists, but the status is deleted hence it cannot be accessed under the id. So like the id is taken but user or admin just cannot print what’s under there cause it was removed
    3. I just didn’t include an endpoint for deleting company since its said I should delete it
    4. There is validation for the length and uniqueness of the values (no two same emails, or pesels for example)
3. Softwares
    1. There is no endpoint for adding new software since it wasn’t required, in the app db context I have seeded some data so it should be able to be enough for testing
    2. There is an endpoint to get all just to make sure they are there
4. Contracts
    1. Contracts just require all the inputs and are initially created, not signed
    2. I have found something on the internet that by itself within a time period checks for expired contracts, hence when a contract expires it notes that, and that money is no longer counted in the revenue, the client can have a new contract for the same software, and overall it kinda just says in memory so we know what happens but we cannot do anything with it
5. Payments
    1. For payment we must input and id and value. As said we can add multiple payments for one contract. It is checked what is the total balance remaining (so like if I already put 50 PLN into 200 PLN contract) and it doesn’t allow for over payment - meaning that if I want to input more then required I will get an error 
    2. For payment there is a field of paymentMethod - its just a string so like credit card or whatever 
6. Revenue 
    1. Current Revenue: Only signed contracts OR actual payments received for created contracts
    2. Predicted Revenue: Full contract value for both signed and created contracts
     3. I hope I understood this correct 
7. Subscriptions
   1. I have started the implementation  but i didnt finish so there are some signs of it, but overall the logic isnt influenced becasue of that 
