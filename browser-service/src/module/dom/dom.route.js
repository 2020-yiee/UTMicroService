import { Router } from 'express';
import * as domController from './dom.controller';

const routes = new Router();

routes.get('/coordinates/:url(*)', domController.getCoordinates);

export default routes;
