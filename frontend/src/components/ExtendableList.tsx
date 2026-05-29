import React from 'react';

const ExtendableList = ({listContent, name}: {listContent: string[], name: string}) => {
    return (
        <div className="flex flex-col mt-2">
            <p className="font-medium pt-2 pb-3 text-disney-blue">{name}</p>
            <ul className={listContent.length > 4 ? 'media-list-container no-scroll': ''}>
                {listContent.map((item, index) => (
                    <li key={index} className="text-disney-blue py-1">{item}</li>
                ))}
            </ul>
        </div>
    )
}

export default ExtendableList;