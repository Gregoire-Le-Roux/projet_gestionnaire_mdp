import jwt_decode from 'jwt-decode';
import { OutilService } from '../services/outil.service';
import { VariableStatic } from './VariableStatic';

type JwtInfo =
{
    iss: string,
    iat: number,
    exp: number
}

export class Jwt
{

    static EstExpirer(): boolean
    {
        const JWT_DECODER: JwtInfo = jwt_decode(VariableStatic.compte.Jwt);

        const DATE = new Date();
        
        return JWT_DECODER.exp < (DATE.getTime() / 1000);
    }
}