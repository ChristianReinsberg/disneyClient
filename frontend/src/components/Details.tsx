import React from 'react';

const Details = ({name, value}: {name: string, value: string}) => {
    return (
        <div className="pt-2">
            <p className="text-disney-blue font-medium">{name}:</p>
            <p className="text-disney-blue">{value}</p>
        </div>
    )
}

export default Details;