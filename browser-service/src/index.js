import express from 'express';
import constants from './config/constants';
import configMiddleware from './config/middlewares';
import routesConfig from './module';

const app = express();

configMiddleware(app);

routesConfig(app);

app.listen(constants.PORT, () =>
  console.log(`
    Service is up on port ${constants.PORT} 🐳
    ---
    // Swagger is running on host/api-docs
    ---`),
);

process.on('SIGINT', () => {
  console.log('Bye bye!');
  process.exit();
});
