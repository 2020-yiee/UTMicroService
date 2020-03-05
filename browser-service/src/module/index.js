import captureRoutes from './capture/capture.route';
import domRoutes from './dom/dom.route';

export default app => {
  app.use('/capture', captureRoutes);
  app.use('/dom', domRoutes);
};
