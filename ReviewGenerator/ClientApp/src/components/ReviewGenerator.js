import React, { Component } from 'react';
import StarRating from "./StarRating";

export class ReviewGenerator extends Component {
    static displayName = ReviewGenerator.name;

    constructor(props) {
        super(props);
        this.state = { review: [] };
        this.generateReview = this.generateReview.bind(this);
    }

    componentDidMount() {
        this.generateReview();
    }

    async generateReview() {
        const response = await fetch('API/generate');
        const data = await response.json();
        this.setState({ review: data });
    }

    render() {
        return (
            <div>
                <button className="btn btn-primary" onClick={this.generateReview}>Generate</button>
                <StarRating starRating={this.state.review.starRating}/>
                <textarea
                    className="txt-area"
                    placeholder="Auto generated review text"
                    value={this.state.review.reviewText}
                    rows={12} cols={75}/>
            </div>
        )
    }
}
