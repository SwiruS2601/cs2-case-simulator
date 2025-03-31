# Google AdSense Implementation

This document outlines how to use the Google AdSense implementation in your CS2 Case Unboxer app.

## Overview

The implementation includes:

1. Ad placeholders for testing ad positions and sizes without actual ads
2. A Google AdSense plugin for loading the AdSense script
3. A production-ready GoogleAd component for real ads
4. Configuration in nuxt.config.ts

## Testing Ad Placements

During development, you can use the `AdPlaceholder` component to visualize where ads will appear:

```vue
<AdPlaceholder size="leaderboard"></AdPlaceholder>
```

The placeholder includes controls to toggle visibility and change ad sizes, making it easy to experiment with different layouts.

## Placeholder Ad Sizes

-   `mobile`: 320×50px - Good for mobile devices and fixed footers
-   `leaderboard`: 320×50px - Same as mobile, standard header/footer size
-   `square`: 250×250px - Good for sidebar or in-content placements
-   `banner`: 728×90px - Wide banner for desktop views

## Switching to Production Ads

When you're ready to deploy real ads, switch from `AdPlaceholder` to `GoogleAd`:

```vue
<GoogleAd adSlot="your-ad-slot-id" size="leaderboard"></GoogleAd>
```

## Creating Ad Units in AdSense

1. Log in to your Google AdSense account
2. Go to Ads → Ad units
3. Create a new ad unit for each placement
4. Note the ad slot ID (e.g., `1234567890`)
5. Use this ID in the `adSlot` prop of the GoogleAd component

## Recommended Ad Placements

Based on testing, these placements provide good balance of revenue and user experience:

1. **Default Layout**

    - One fixed banner at the bottom of the screen (mobile size)
    - One banner between title and content on main pages

2. **Crate Opening Page**

    - One square ad unit under the crate details
    - One leaderboard under the case opening result

3. **Inventory Page**
    - One leaderboard at the top
    - One leaderboard between pagination controls

## Performance Optimization

The implementation includes these optimizations:

-   Lazy loading using Intersection Observer
-   Ads only load when they become visible
-   Client-side only rendering via `<ClientOnly>`
-   Proper TypeScript typing for AdSense

## AdSense Responsive Ads

By default, all ad units are set to responsive mode. This can be toggled with the `isResponsive` prop:

```vue
<GoogleAd adSlot="your-ad-slot-id" :isResponsive="false"></GoogleAd>
```

## Troubleshooting

1. If ads aren't showing, check the browser console for errors
2. Verify that your AdSense account is approved
3. Make sure your ad slots match the IDs in your AdSense account
4. Allow 24-48 hours for new ad units to start showing ads

## Google AdSense Policies

Remember to comply with Google AdSense policies:

1. Don't place more than 3 ads per page on mobile
2. Don't encourage users to click on ads
3. Don't place ads in a way that would result in accidental clicks
4. Allow 24-48 hours for Google to crawl and approve your site after implementation

## Further Customization

The GoogleAd component accepts these props:

-   `adSlot`: (required) Your ad unit ID
-   `adFormat`: Format of the ad (default: 'auto')
-   `adClient`: Your publisher ID (default: configured in the component)
-   `size`: Predefined size (banner, square, leaderboard, mobile)
-   `isResponsive`: Whether the ad should adapt to container (default: true)
-   `isFixed`: Whether the ad should be fixed to screen (default: false)
-   `customStyle`: Additional CSS styles to apply
