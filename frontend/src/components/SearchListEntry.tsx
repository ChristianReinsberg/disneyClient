import React from 'react'
import { Link } from 'react-router-dom';
import { SearchEntry } from '../types';
import { to } from '../consts';

const SearchListEntry = ({entries, title} : {entries: SearchEntry[], title: string}) => {
    if (entries.length === 0) {
        return null;
    }
    return (
        <div className="flex flex-col">
            <h2 className="text-2xl font-medium text-disney-blue my-4">{title}</h2>
            {entries.map(entry => {
                return (
                    <Link key={entry.id} className="text-disney-blue font-medium odd:bg-gray-100 p-4" to={`/${to[entry.type] + entry.id}`}>{entry.name}</Link>
                )
            })}
            {title !== 'Series' ? (<hr />) : null}
        </div>
    )
}

export default SearchListEntry;