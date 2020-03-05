const devConfig = {};

const prodConfig = {};

const defaultConfig = {
  PORT: process.env.PORT || 7777,
};

function envConfig(env) {
  switch (env) {
    case 'dev':
      return devConfig;
    default:
      return prodConfig;
  }
}

export default {
  ...defaultConfig,
  ...envConfig(process.env.NODE_ENV),
  BROWSER_CONFIG: {
    width: 1280,
    height: 800,
    scaleFactor: 1,
  },
  CONNECT_OPTIONS: {
    timeout: 60 * 1000,
    waitUntil: 'networkidle2',
  },
};
