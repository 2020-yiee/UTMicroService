const path = require('path');
const puppeteer = require('puppeteer');

(async () => {
  const { webID, trackingID, Url } = {
    webID: 'webID',
    trackingID: 'trackingID',
    Url: 'h/www.google.com/',
  };
  const browser = await puppeteer.launch();
  const page = await browser.newPage();

  await page.setJavaScriptEnabled(true);
  await page.setViewport({
    width: 1280,
    height: 800,
    scaleFactor: 1,
  });

  const response = await page.goto(Url, {
    timeout: 1 * 1000,
    waitUntil: 'load',
  });
  console.log({ response });

  const imagePath = path.join(__dirname, `images/${webID}/${trackingID}.png`);
  console.log(imagePath);
  await page.screenshot({
    path: imagePath,
    type: 'png',
    fullPage: true,
  });

  await page.close();
  await browser.close();
})();
