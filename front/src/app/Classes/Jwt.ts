import jwt_decode from 'jwt-decode';
import { ExportCompte } from '../Types/Export/ExportCompte';

type JwtInfo =
{
    iss: string,
    iat: number,
    exp: number
}


export class Jwt
{
    private token: string;

    constructor(_token: string) 
    {
        this.token = _token;
    }

    EstValide(): boolean
    {
        const JWT_DECODER: JwtInfo = jwt_decode(this.token);

        const DATE = new Date();

        return JWT_DECODER.exp > (DATE.getTime() / 1000);
    }

    InfoToken(): ExportCompte
    {
        const JWT_DECODER: ExportCompte = jwt_decode(this.token);

        return JWT_DECODER;
    }
}