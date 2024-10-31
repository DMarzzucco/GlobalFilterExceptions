import {Request, Response, NextFunction} from "express";

interface CustomError extends Error {
    statusCode?:number;
    errorCode?:string;
}

function logError( error: CustomError ){ console.error(error.stack); }


export function GlobalFilterException (
    err:CustomError,
    _req:Request,
    res:Response,
    _next:NextFunction
){
    logError(err);

    const statusCode = err.statusCode || 500;
    const message = err.message || `Some internal Error Server: ${err.message}`;

    return res.status(statusCode).json({
	status:"error",
	statusCode,
	message,
	errorCode:err.name
    })
}
