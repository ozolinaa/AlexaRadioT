docker run -d -e WebApplicationUrl='https://radio-t.lalala.space' --name alexaradiot --restart=unless-stopped -p 8080:80 xtonyx/alexaradiot:20180605065851

https://www.digitalocean.com/community/tutorials/how-to-install-and-use-docker-on-ubuntu-16-04
https://www.digitalocean.com/community/tutorials/how-to-secure-nginx-with-let-s-encrypt-on-ubuntu-16-04
https://stackoverflow.com/questions/28572392/nginx-serve-from-static-if-not-found-try-reverse-proxy

sudo certbot --nginx -d radio-t.lalala.space
sudo certbot renew --dry-run