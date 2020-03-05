/* eslint-disable no-unused-vars */
import HTTPStatus from 'http-status';
import path from 'path';
import puppeteer from 'puppeteer';
import mkdirp from 'mkdirp';

import constants from '../../config/constants';

export async function getImage(req, res) {
  const { webID, trackingID } = req.params;

  try {
    const imageFolder = path.join(__dirname, `../../../images/${webID}`);
    const imagePath = path.join(imageFolder, `${trackingID}.png`);
    return res.status(HTTPStatus.OK).sendFile(imagePath);
  } catch (e) {
    return res.sendStatus(HTTPStatus.BAD_REQUEST);
  }
}

export async function captureImage(req, res) {
  const { webID, trackingID, url } = req.params;

  try {
    const browser = await puppeteer.launch();
    const page = await browser.newPage();

    await page.setJavaScriptEnabled(true);
    await page.setViewport(constants.BROWSER_CONFIG);
    await page.goto(url, constants.CONNECT_OPTIONS);

    const imageFolder = path.join(__dirname, `../../../images/${webID}`);
    const imagePath = path.join(imageFolder, `${trackingID}.png`);
    await mkdirp(imageFolder);

    await page.screenshot({
      path: imagePath,
      type: 'png',
      fullPage: true,
    });

    await page.close();
    await browser.close();

    return res.sendStatus(HTTPStatus.OK);
  } catch (e) {
    return res.sendStatus(HTTPStatus.BAD_REQUEST);
  }
}
