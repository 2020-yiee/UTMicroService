/* eslint-disable no-nested-ternary */
import passport from 'passport';
import HTTPStatus from 'http-status';
import LocalStrategy from 'passport-local';
import { Strategy as JWTStrategy, ExtractJwt } from 'passport-jwt';
import constants from '../config/constants';
import Employee from '../module/employee/employee.model';
import Admin from '../module/admin/admin.model';
import Company from '../module/company/company.model';

const localOpts = {
  usernameField: 'username',
  passReqToCallback: true,
};
const localStrategy = new LocalStrategy(localOpts, async (req, username, password, done) => {
  try {
    const user = (!req.baseUrl.includes('admins')) ? await Employee.findOne({ username, isRemoved: false }) :
      await Admin.findOne({ username, isRemoved: false });

    if (!user) {
      return done(null, false);
    } else if (!user.validatePassword(password)) {
      return done(null, false);
    }

    return done(null, user);
  } catch (error) {
    return done(error, false);
  }
});
passport.use(localStrategy);

export const authLocal = async (req, res, next) => passport.authenticate('local', { session: false },
  async (err, user) => {
    if (!user) {
      return res.status(HTTPStatus.BAD_REQUEST).json('Invalid username or password');
    }

    if (user.role) {
      if (!user.isConfirmed) { return res.status(HTTPStatus.PRECONDITION_REQUIRED).json('Please confirm your email to login'); }
      const company = await Company.findOne({ _id: user.company });
      return res.status(HTTPStatus.OK).json({ ...user.toAuthJSON(), company: company.toJSON() });
    }

    return res.status(HTTPStatus.OK).json(user.toAuthJSON());
  })(req, res, next);

//------------------------------------

const jwtOpts = {
  jwtFromRequest: ExtractJwt.fromExtractors([
    ExtractJwt.fromHeader('token'),
    (req) => req.params.token,
  ]),
  secretOrKey: constants.JWT_SECRET,
};

const jwtStrategy = new JWTStrategy(jwtOpts, async (payload, done) => {
  try {
    const user = (payload.role == 'employee') ? await Employee.findOne({ _id: payload._id, isRemoved: false }) : 
      (payload.role == 'company') ? await Company.findOne({ _id: payload._id, isRemoved: false }) : 
        await Admin.findOne({ _id: payload._id, isRemoved: false });
    if (!user) {
      return done(null, false);
    }
    return done(null, user);
  } catch (e) {
    return done(e, false);
  }
});

passport.use(jwtStrategy);
export const authJwt = passport.authenticate('jwt', { session: false });
