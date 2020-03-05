import { Router } from 'express';
import * as captureController from './capture.controller';

const routes = new Router();

routes.get('/:webID/:trackingID/', captureController.getImage);
routes.head('/:webID/:trackingID/:url(*)', captureController.captureImage);

export default routes;
