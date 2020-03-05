/* eslint-disable no-shadow */
/* eslint-disable no-unused-vars */
import HTTPStatus from 'http-status';
import puppeteer from 'puppeteer';

import constants from '../../config/constants';

export async function getCoordinates(req, res) {
  const { url } = req.params;
  const { events } = req.body;

  try {
    const browser = await puppeteer.launch();
    const page = await browser.newPage();

    await page.setJavaScriptEnabled(true);
    await page.setViewport(constants.BROWSER_CONFIG);
    await page.goto(url, constants.CONNECT_OPTIONS);

    const coordinates = await Promise.all(
      events.map(
        async ({
          elementSelector,
          clientOffsetX,
          clientOffsetY,
          clientWidth,
          clientHeight,
        }) => {
          const { left, top, width, height } = await page.evaluate(selector => {
            function getDocumentOffsetPosition(element) {
              const position = {
                top: element.offsetTop,
                left: element.offsetLeft,
              };

              if (element.offsetParent) {
                const parentPosition = getDocumentOffsetPosition(
                  element.offsetParent,
                );
                position.top += parentPosition.top;
                position.left += parentPosition.left;
              }
              return position;
            }
            const node = document.querySelector(selector);
            const { offsetWidth: width, offsetHeight: height } = node;
            const { left, top } = getDocumentOffsetPosition(node);
            return { left, top, width, height };
          }, elementSelector);

          const offsetX = (clientOffsetX / clientWidth) * width;
          const offsetY = (clientOffsetY / clientHeight) * height;

          return {
            x: left + offsetX,
            y: top + offsetY,
          };
        },
      ),
    );

    await page.close();
    await browser.close();

    return res.status(HTTPStatus.OK).json(coordinates);
  } catch (e) {
    return res.status(HTTPStatus.BAD_REQUEST).json(e.message);
  }
}
