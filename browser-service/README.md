# Smart Accountant System

## Development

```bash
npm install && npm run dev
```

or

```bash
yarn && yarn dev
```

## Test API

0. Run development
1. Import the following JSON into `SAS-service-dev` > `users`:

```JSON
{
    "_id" : ObjectId("5cf54198e88ac320c0014e1f"),
    "isRemoved" : false,
    "username" : "thor",
    "password" : "$2a$10$DpYIO6e83j3Iup8jaHVq/uQg8hX2P0G9ZhS6mlKvC0C9QhIoC11tK",
    "fullname" : "duke thor",
    "phone" : "1234567890",
    "createdAt" : ISODate("2019-06-03T15:49:44.276+0000"),
    "updatedAt" : ISODate("2019-06-03T15:49:44.276+0000"),
    "__v" : NumberInt(0)
}
```

2. Import `SAS.postman_collection.json` to Postman
3. Use `(username, password) = (quang, 123456)` to login & get token
4. Feel free to test API & remember to update `SAS.postman_collection.json`

## Requires

- NodeJS
- Postman
- Yarn (optional)
