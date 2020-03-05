import bodyParser from 'body-parser';
import compression from 'compression';
import helmet from 'helmet';
import cors from 'cors';
import passport from 'passport';
import morgan from 'morgan';
// import swaggerUi from 'swagger-ui-express';

const isProd = process.env.NODE_ENV != 'dev';

export default app => {
  if (isProd) {
    app.use(compression());
    app.use(helmet());
  }

  app.use(morgan('tiny'));
  app.use(bodyParser.json({ limit: '5mb' }));
  app.use(bodyParser.urlencoded({ extended: true }));
  app.use(passport.initialize());
  app.use(cors());
};
