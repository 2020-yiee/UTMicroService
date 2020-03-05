import HTTPStatus from 'http-status';
import constants from '../config/constants';

// NOTA: NOTADMIN
export function roleNOTA(req, res, next) {
  if (req.user.role) {
    return next();
  }
  return res.sendStatus(HTTPStatus.FORBIDDEN);
}


// NOTS: NOTSTAFF
export function roleNOTS(req, res, next) {
  if (req.user.role != constants.ROLE.STAFF) {
    return next();
  }
  return res.sendStatus(HTTPStatus.FORBIDDEN);
}

// NOTS: NOT ACCOUNTANT
export function roleNOTA2(req, res, next) {
  if (req.user.role != constants.ROLE.ACCOUNTANT) {
    return next();
  }
  return res.sendStatus(HTTPStatus.FORBIDDEN);
}


export function roleNOTO(req, res, next) {
  if (req.user._id !== req.params.id) {
    return next();
  }
  return res.sendStatus(HTTPStatus.FORBIDDEN);
}


export function roleAdmin(req, res, next) {
  if (!req.user.role) {
    return next();
  }
  return res.sendStatus(HTTPStatus.FORBIDDEN);
}

export function roleAccountant(req, res, next) {
  if (req.user.role == constants.ROLE.ACCOUNTANT) {
    return next();
  }
  return res.sendStatus(HTTPStatus.FORBIDDEN);
}

export function roleManager(req, res, next) {
  if (req.user.role == constants.ROLE.MANAGER) {
    return next();
  }
  return res.sendStatus(HTTPStatus.FORBIDDEN);
}

export function roleStaff(req, res, next) {
  if (req.user.role == constants.ROLE.STAFF) {
    return next();
  }
  return res.sendStatus(HTTPStatus.FORBIDDEN);
}
  

export function roleOwn(req, res, next) {
  if (req.user._id == req.params.id) {
    return next();
  }
  return res.sendStatus(HTTPStatus.FORBIDDEN);
}

// AM: Admin or manager
export function roleAM(req, res, next) {
  if (!req.user.role || req.user.role == constants.ROLE.MANAGER) {
    return next();
  }
  return res.sendStatus(HTTPStatus.FORBIDDEN);
}

// AMO: Admin, manager or own
export function roleAMO(req, res, next) {
  if (!req.user.role || req.user.role == constants.ROLE.MANAGER || req.user._id == req.params.id) {
    return next();
  }
  return res.sendStatus(HTTPStatus.FORBIDDEN);
}
