import React from 'react';

const Pagination = ({hasNext, hasPrev, update}: {hasNext: boolean, hasPrev: boolean, update: Function}) => {
    const loadNext = (event : Event) => {
        event.preventDefault();
        update(true);
    };
    const loadPrev = (event : Event) => {
        event.preventDefault();
        update(false);
    }
    return (<div className={`mt-8 flex ${hasPrev ? 'justify-between' : 'justify-end'}`}>
        <button onClick={loadPrev} className={`border border-gray-500 p-2 rounded-lg cursor-pointer ${!hasPrev ? 'hidden' : ''}`}>prev page</button>
        <button onClick={loadNext} className={`border border-gray-500 p-2 rounded-lg cursor-pointer ${!hasNext ? 'hidden' : ''}`}>next page</button>
    </div>)
}

export default Pagination;